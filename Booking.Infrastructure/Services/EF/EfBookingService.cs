using Booking.Application.Abstractions;
using Booking.Contracts.Dtos.Bookings;
using Booking.Contracts.Requests.Bookings;
using Booking.Domain.Bookings;
using Booking.Infrastructure.Mappers;
using Booking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Services.Ef;

public class EfBookingService(BookingDbContext dbContext) : IBookingService
{
    private static readonly string[] BlockingStatuses =
    [
        "pending_owner_approval",
        "confirmed"
    ];

    public async Task<IReadOnlyList<BookingDto>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var bookings = await dbContext.Bookings
            .AsNoTracking()
            .Where(x => x.UserId == userId && !x.IsHiddenForUser)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return bookings.Select(x => x.ToDto()).ToList();
    }

    public async Task<IReadOnlyList<BookingDto>> GetUpcomingByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var bookings = await dbContext.Bookings
            .AsNoTracking()
            .Where(x =>
                x.UserId == userId &&
                !x.IsHiddenForUser &&
                x.CheckOut >= today &&
                x.Status != "cancelled_by_guest" &&
                x.Status != "cancelled_by_owner" &&
                x.Status != "rejected_by_owner")
            .OrderBy(x => x.CheckIn)
            .ToListAsync(cancellationToken);

        return bookings.Select(x => x.ToDto()).ToList();
    }

    public async Task<IReadOnlyList<BookingDto>> GetHistoryByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var bookings = await dbContext.Bookings
            .AsNoTracking()
            .Where(x =>
                x.UserId == userId &&
                !x.IsHiddenForUser &&
                (
                    x.CheckOut < today ||
                    x.Status == "cancelled_by_guest" ||
                    x.Status == "cancelled_by_owner" ||
                    x.Status == "rejected_by_owner"
                ))
            .OrderByDescending(x => x.CheckOut)
            .ToListAsync(cancellationToken);

        return bookings.Select(x => x.ToDto()).ToList();
    }

    public async Task<BookingDto?> GetByIdForUserAsync(
        string userId,
        string bookingId,
        CancellationToken cancellationToken = default)
    {
        var booking = await dbContext.Bookings
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == bookingId &&
                     x.UserId == userId &&
                     !x.IsHiddenForUser,
                cancellationToken);

        return booking?.ToDto();
    }

    public async Task<BookingDto> CreateAsync(
        string userId,
        CreateBookingRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new InvalidOperationException("User id is required.");
        }

        var userExists = await dbContext.Users
            .AnyAsync(x => x.Id == userId && !x.IsBlocked, cancellationToken);

        if (!userExists)
        {
            throw new InvalidOperationException("User not found or blocked.");
        }

        if (!DateOnly.TryParse(request.CheckIn, out var checkIn))
        {
            throw new InvalidOperationException("Invalid check-in date.");
        }

        if (!DateOnly.TryParse(request.CheckOut, out var checkOut))
        {
            throw new InvalidOperationException("Invalid check-out date.");
        }

        if (checkOut <= checkIn)
        {
            throw new InvalidOperationException("Check-out date must be after check-in date.");
        }

        if (request.Guests <= 0)
        {
            throw new InvalidOperationException("Guests count must be greater than zero.");
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (checkIn < today)
        {
            throw new InvalidOperationException("Check-in date cannot be in the past.");
        }

        var hotel = await dbContext.Hotels
            .Include(x => x.Rooms.Where(r => !r.IsDeleted))
            .FirstOrDefaultAsync(
                x => x.Id == request.HotelId && !x.IsDeleted,
                cancellationToken);

        if (hotel is null)
        {
            throw new InvalidOperationException("Hotel not found.");
        }

        var room = hotel.Rooms.FirstOrDefault(x => x.Id == request.RoomId);

        if (room is null)
        {
            throw new InvalidOperationException("Room not found.");
        }

        if (hotel.OwnerUserId == userId)
        {
            throw new InvalidOperationException("You cannot book your own hotel.");
        }

        var roomIsAvailable = await IsRoomAvailableAsync(
            room.Id,
            checkIn,
            checkOut,
            cancellationToken);

        if (!roomIsAvailable)
        {
            throw new InvalidOperationException("Room is not available for selected dates.");
        }

        var nights = checkOut.DayNumber - checkIn.DayNumber;
        var totalPrice = room.Price * nights;

        var status = string.IsNullOrWhiteSpace(hotel.OwnerUserId)
            ? "confirmed"
            : "pending_owner_approval";

        var booking = new HotelBooking
        {
            Id = $"bok_{Guid.NewGuid():N}"[..12],
            UserId = userId,
            HotelOwnerUserId = hotel.OwnerUserId,

            HotelId = hotel.Id,
            RoomId = room.Id,

            CheckIn = checkIn,
            CheckOut = checkOut,

            Guests = request.Guests,

            Status = status,

            TotalPrice = totalPrice,
            Currency = string.IsNullOrWhiteSpace(request.Currency)
                ? "USD"
                : request.Currency,

            IsHiddenForUser = false,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Bookings.Add(booking);

        await dbContext.SaveChangesAsync(cancellationToken);

        return booking.ToDto();
    }

    public async Task<bool> CancelAsync(
        string userId,
        string bookingId,
        CancellationToken cancellationToken = default)
    {
        var booking = await dbContext.Bookings
            .FirstOrDefaultAsync(
                x => x.Id == bookingId &&
                     x.UserId == userId,
                cancellationToken);

        if (booking is null)
        {
            return false;
        }

        if (booking.Status is "cancelled_by_guest" or "cancelled_by_owner" or "rejected_by_owner")
        {
            return true;
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (booking.CheckIn <= today)
        {
            throw new InvalidOperationException("Booking can no longer be cancelled by guest.");
        }

        booking.Status = "cancelled_by_guest";
        booking.CancelledAtUtc = DateTime.UtcNow;
        booking.CancellationReason = "Cancelled by guest.";

        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> RestoreAsync(
        string userId,
        string bookingId,
        CancellationToken cancellationToken = default)
    {
        /*
         * В новом workflow я НЕ давал пользователю restore.
         * Почему: если бронь была отменена, комната могла стать доступной другим.
         */
        throw new InvalidOperationException("Booking restore is not supported. Please create a new booking.");
    }

    public async Task<bool> HideForUserAsync(
        string userId,
        string bookingId,
        CancellationToken cancellationToken = default)
    {
        var booking = await dbContext.Bookings
            .FirstOrDefaultAsync(
                x => x.Id == bookingId &&
                     x.UserId == userId,
                cancellationToken);

        if (booking is null)
        {
            return false;
        }

        booking.IsHiddenForUser = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private async Task<bool> IsRoomAvailableAsync(
        string roomId,
        DateOnly checkIn,
        DateOnly checkOut,
        CancellationToken cancellationToken)
    {
        var hasOverlap = await dbContext.Bookings
            .AnyAsync(
                x => x.RoomId == roomId &&
                     BlockingStatuses.Contains(x.Status) &&
                     x.CheckIn < checkOut &&
                     checkIn < x.CheckOut,
                cancellationToken);

        return !hasOverlap;
    }
}