using Booking.Contracts.Common;
using Booking.Contracts.Dtos.Bookings;

namespace Booking.Application.Abstractions;

public interface IOwnerBookingService
{
    Task<PagedResult<BookingDto>> GetAllForOwnerAsync(
        string ownerUserId,
        PaginationRequest pagination,
        string? status = null,
        CancellationToken cancellationToken = default);

    Task<PagedResult<BookingDto>> GetPendingForOwnerAsync(
        string ownerUserId,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default);

    Task<BookingDto?> GetByIdForOwnerAsync(
        string ownerUserId,
        string bookingId,
        CancellationToken cancellationToken = default);

    Task<BookingDto?> AcceptAsync(
        string ownerUserId,
        string bookingId,
        CancellationToken cancellationToken = default);

    Task<BookingDto?> RejectAsync(
        string ownerUserId,
        string bookingId,
        string? reason = null,
        CancellationToken cancellationToken = default);

    Task<BookingDto?> CancelAsync(
        string ownerUserId,
        string bookingId,
        string? reason = null,
        CancellationToken cancellationToken = default);
}