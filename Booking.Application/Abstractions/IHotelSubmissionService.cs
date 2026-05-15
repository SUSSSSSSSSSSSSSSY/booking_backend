using Booking.Contracts.Common;
using Booking.Contracts.Dtos.Hotels;
using Booking.Contracts.Requests.Hotels;

namespace Booking.Application.Abstractions;

public interface IHotelSubmissionService
{
    Task<HotelSubmissionDto> CreateAsync(
        string userId,
        CreateHotelSubmissionRequest request,
        CancellationToken cancellationToken = default);

    Task<PagedResult<HotelSubmissionDto>> GetMySubmissionsAsync(
        string userId,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default);

    Task<HotelSubmissionDto?> GetMySubmissionByIdAsync(
        string userId,
        string submissionId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<HotelDto>> GetMyHotelsAsync(
        string userId,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default);
    Task<HotelSubmissionDto> SubmitUpdateAsync(
        string userId,
        string hotelId,
        UpdateOwnedHotelSubmissionRequest request,
        CancellationToken cancellationToken = default);

    Task<HotelSubmissionDto> SubmitDeleteAsync(
        string userId,
        string hotelId,
        DeleteOwnedHotelSubmissionRequest request,
        CancellationToken cancellationToken = default);
}