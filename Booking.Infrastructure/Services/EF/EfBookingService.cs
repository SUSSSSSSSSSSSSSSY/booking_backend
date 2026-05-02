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
    public async Task<IReadOnlyList<BookingDto>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var bookings = await dbContext.Bookings
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return bookings
            .Select(x => x.ToDto())
            .ToList();
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

        var nights = checkOut.DayNumber - checkIn.DayNumber;
        var totalPrice = room.Price * nights;

        var booking = new HotelBooking
        {
            Id = $"bok_{Guid.NewGuid():N}"[..12],
            UserId = userId,
            HotelId = hotel.Id,
            RoomId = room.Id,
            CheckIn = checkIn,
            CheckOut = checkOut,
            Guests = request.Guests,
            Status = "confirmed",
            TotalPrice = totalPrice,
            Currency = string.IsNullOrWhiteSpace(request.Currency)
                ? "USD"
                : request.Currency,
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
                x => x.Id == bookingId && x.UserId == userId,
                cancellationToken);

        if (booking is null)
        {
            return false;
        }

        if (booking.Status == "cancelled")
        {
            return true;
        }

        booking.Status = "cancelled";

        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}