using Booking.Contracts.Dtos.Admin;

namespace Booking.Application.Abstractions.Admin;

public interface IAdminBookingService
{
    Task<IReadOnlyList<AdminBookingDto>> GetAllAsync(
        string? status = null,
        CancellationToken cancellationToken = default);

    Task<AdminBookingDto?> GetByIdAsync(
        string bookingId,
        CancellationToken cancellationToken = default);

    Task<AdminBookingDto?> CancelAsync(
        string bookingId,
        CancellationToken cancellationToken = default);
}