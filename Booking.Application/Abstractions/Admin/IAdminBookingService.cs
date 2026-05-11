using Booking.Contracts.Common;
using Booking.Contracts.Dtos.Admin;

namespace Booking.Application.Abstractions.Admin;

public interface IAdminBookingService
{
    Task<PagedResult<AdminBookingDto>> GetAllAsync(
        PaginationRequest pagination,
        string? status = null,
        CancellationToken cancellationToken = default);

    Task<AdminBookingDto?> GetByIdAsync(
        string bookingId,
        CancellationToken cancellationToken = default);

    Task<AdminBookingDto?> CancelAsync(
        string bookingId,
        CancellationToken cancellationToken = default);
}