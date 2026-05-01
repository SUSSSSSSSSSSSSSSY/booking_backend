using Booking.Contracts.Dtos.Hotels;
using Booking.Contracts.Requests.Admin;

namespace Booking.Application.Abstractions.Admin;

public interface IAdminHotelService
{
    Task<IReadOnlyList<HotelDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<HotelDto?> GetByIdAsync(string hotelId, CancellationToken cancellationToken = default);

    Task<HotelDto> CreateAsync(CreateHotelRequest request, CancellationToken cancellationToken = default);
    Task<HotelDto?> UpdateAsync(string hotelId, UpdateHotelRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string hotelId, CancellationToken cancellationToken = default);

    Task<HotelDto?> AddRoomAsync(string hotelId, CreateRoomRequest request, CancellationToken cancellationToken = default);
    Task<HotelDto?> UpdateRoomAsync(string hotelId, string roomId, UpdateRoomRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteRoomAsync(string hotelId, string roomId, CancellationToken cancellationToken = default);
}