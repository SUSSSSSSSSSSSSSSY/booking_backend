using Booking.Contracts.Common;
using Booking.Contracts.Dtos.Hotels;

namespace Booking.Application.Abstractions;

public interface IHotelService
{
    Task<PagedResult<HotelDto>> GetAllAsync(
        PaginationRequest pagination,
        CancellationToken cancellationToken = default);

    Task<HotelDto?> GetByIdAsync(
        string hotelId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<HotelDto>> SearchAsync(
        string? city,
        string? country,
        CancellationToken cancellationToken = default);
}