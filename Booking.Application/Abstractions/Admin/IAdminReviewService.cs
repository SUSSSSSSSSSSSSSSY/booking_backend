using Booking.Contracts.Dtos.Admin;

namespace Booking.Application.Abstractions.Admin;

public interface IAdminReviewService
{
    Task<IReadOnlyList<AdminReviewDto>> GetAllAsync(
        string? hotelId = null,
        CancellationToken cancellationToken = default);

    Task<AdminReviewDto?> GetByIdAsync(
        string reviewId,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        string reviewId,
        CancellationToken cancellationToken = default);
}