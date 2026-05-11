using Booking.Contracts.Common;
using Booking.Contracts.Dtos.Admin;

namespace Booking.Application.Abstractions.Admin;

public interface IAdminReviewService
{
    Task<PagedResult<AdminReviewDto>> GetAllAsync(
        PaginationRequest pagination,
        string? hotelId = null,
        CancellationToken cancellationToken = default);

    Task<AdminReviewDto?> GetByIdAsync(
        string reviewId,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        string reviewId,
        CancellationToken cancellationToken = default);
}