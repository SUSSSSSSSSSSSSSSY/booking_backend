using Booking.Contracts.Dtos.Bookings;
using Booking.Contracts.Requests.Bookings;

namespace Booking.Application.Abstractions;

public interface IBookingService
{
    Task<IReadOnlyList<BookingDto>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BookingDto>> GetUpcomingByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BookingDto>> GetHistoryByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<BookingDto?> GetByIdForUserAsync(
        string userId,
        string bookingId,
        CancellationToken cancellationToken = default);

    Task<BookingDto> CreateAsync(
        string userId,
        CreateBookingRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> CancelAsync(
        string userId,
        string bookingId,
        CancellationToken cancellationToken = default);

    Task<bool> RestoreAsync(
        string userId,
        string bookingId,
        CancellationToken cancellationToken = default);

    Task<bool> HideForUserAsync(
        string userId,
        string bookingId,
        CancellationToken cancellationToken = default);
}