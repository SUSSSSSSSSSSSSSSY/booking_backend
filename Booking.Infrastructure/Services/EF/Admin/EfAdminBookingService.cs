using Booking.Application.Abstractions.Admin;
using Booking.Contracts.Dtos.Admin;
using Booking.Domain.Bookings;
using Booking.Infrastructure.Mappers;
using Booking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Services.Ef.Admin;

public class EfAdminBookingService(BookingDbContext dbContext) : IAdminBookingService
{
    public async Task<IReadOnlyList<AdminBookingDto>> GetAllAsync(
        string? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Bookings
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.Trim().ToLower();
            query = query.Where(x => x.Status.ToLower() == normalizedStatus);
        }

        var bookings = await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return await MapBookingsAsync(bookings, cancellationToken);
    }

    public async Task<AdminBookingDto?> GetByIdAsync(
        string bookingId,
        CancellationToken cancellationToken = default)
    {
        var booking = await dbContext.Bookings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == bookingId, cancellationToken);

        if (booking is null)
        {
            return null;
        }

        return await MapBookingAsync(booking, cancellationToken);
    }

    public async Task<AdminBookingDto?> CancelAsync(
        string bookingId,
        CancellationToken cancellationToken = default)
    {
        var booking = await dbContext.Bookings
            .FirstOrDefaultAsync(x => x.Id == bookingId, cancellationToken);

        if (booking is null)
        {
            return null;
        }

        booking.Status = "cancelled";

        await dbContext.SaveChangesAsync(cancellationToken);

        return await MapBookingAsync(booking, cancellationToken);
    }

    private async Task<List<AdminBookingDto>> MapBookingsAsync(
        List<HotelBooking> bookings,
        CancellationToken cancellationToken)
    {
        var result = new List<AdminBookingDto>();

        foreach (var booking in bookings)
        {
            var dto = await MapBookingAsync(booking, cancellationToken);

            if (dto is not null)
            {
                result.Add(dto);
            }
        }

        return result;
    }

    private async Task<AdminBookingDto?> MapBookingAsync(
        HotelBooking booking,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == booking.UserId, cancellationToken);

        var hotel = await dbContext.Hotels
            .AsNoTracking()
            .Include(x => x.Rooms)
            .FirstOrDefaultAsync(x => x.Id == booking.HotelId, cancellationToken);

        var room = hotel?.Rooms.FirstOrDefault(x => x.Id == booking.RoomId);

        return booking.ToAdminDto(user, hotel, room);
    }
}