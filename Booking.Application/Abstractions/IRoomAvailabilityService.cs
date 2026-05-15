using Booking.Contracts.Dtos.Bookings;

namespace Booking.Application.Abstractions;

public interface IRoomAvailabilityService
{
    Task<RoomAvailabilityDto?> GetRoomAvailabilityAsync(
        string roomId,
        string? checkIn,
        string? checkOut,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RoomAvailabilityDto>> GetHotelRoomsAvailabilityAsync(
        string hotelId,
        string? checkIn,
        string? checkOut,
        CancellationToken cancellationToken = default);
}