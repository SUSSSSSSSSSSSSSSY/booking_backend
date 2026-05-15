using Booking.Contracts.Common;
using Booking.Contracts.Dtos.Hotels;
using Booking.Contracts.Requests.Admin;

namespace Booking.Application.Abstractions.Admin;

public interface IAdminHotelSubmissionService
{
    Task<PagedResult<HotelSubmissionDto>> GetAllAsync(
        PaginationRequest pagination,
        string? status = null,
        CancellationToken cancellationToken = default);

    Task<HotelSubmissionDto?> GetByIdAsync(
        string submissionId,
        CancellationToken cancellationToken = default);

    Task<HotelSubmissionDto?> ApproveAsync(
        string adminUserId,
        string submissionId,
        CancellationToken cancellationToken = default);

    Task<HotelSubmissionDto?> RejectAsync(
        string adminUserId,
        string submissionId,
        RejectHotelSubmissionRequest request,
        CancellationToken cancellationToken = default);
}