using Booking.Contracts.Common;
using Booking.Contracts.Dtos.Hotels;
using Booking.Contracts.Requests.Hotels;

namespace Booking.Application.Abstractions;

public interface IHotelService
{
    Task<PagedResult<HotelDto>> GetAllAsync(
        PaginationRequest pagination,
        CancellationToken cancellationToken = default);

    Task<HotelDto?> GetByIdAsync(
        string hotelId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<HotelDto>> SearchAsync(
        HotelSearchRequest request,
        CancellationToken cancellationToken = default);
}