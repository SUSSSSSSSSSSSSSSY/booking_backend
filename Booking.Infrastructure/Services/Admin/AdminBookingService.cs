using Booking.Application.Abstractions.Admin;
using Booking.Contracts.Common;
using Booking.Contracts.Dtos.Admin;
using Booking.Infrastructure.Mappers;
using Booking.Infrastructure.Storage;

namespace Booking.Infrastructure.Services.Admin;

public class AdminBookingService(InMemoryStore store) : IAdminBookingService
{
    public Task<PagedResult<AdminBookingDto>> GetAllAsync(
    PaginationRequest pagination,
    string? status = null,
    CancellationToken cancellationToken = default)
    {
        pagination.Normalize();

        var query = store.Bookings.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x =>
                x.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
        }

        query = query.OrderByDescending(x => x.CreatedAtUtc);

        var totalCount = query.Count();

        var items = query
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .Select(MapBooking)
            .ToList();

        var result = PagedResult<AdminBookingDto>.Create(
            items,
            pagination.Page,
            pagination.PageSize,
            totalCount);

        return Task.FromResult(result);
    }

    public Task<AdminBookingDto?> GetByIdAsync(
        string bookingId,
        CancellationToken cancellationToken = default)
    {
        var booking = store.Bookings.FirstOrDefault(x => x.Id == bookingId);

        if (booking is null)
        {
            return Task.FromResult<AdminBookingDto?>(null);
        }

        return Task.FromResult<AdminBookingDto?>(MapBooking(booking));
    }

    public Task<AdminBookingDto?> CancelAsync(
        string bookingId,
        CancellationToken cancellationToken = default)
    {
        var booking = store.Bookings.FirstOrDefault(x => x.Id == bookingId);

        if (booking is null)
        {
            return Task.FromResult<AdminBookingDto?>(null);
        }

        booking.Status = "cancelled";

        return Task.FromResult<AdminBookingDto?>(MapBooking(booking));
    }

    private AdminBookingDto MapBooking(Booking.Domain.Bookings.HotelBooking booking)
    {
        var user = store.Users.FirstOrDefault(x => x.Id == booking.UserId);
        var hotel = store.Hotels.FirstOrDefault(x => x.Id == booking.HotelId);
        var room = hotel?.Rooms.FirstOrDefault(x => x.Id == booking.RoomId);

        return booking.ToAdminDto(user, hotel, room);
    }
}