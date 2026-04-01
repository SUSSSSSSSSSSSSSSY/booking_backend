using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Booking.Contracts.Dtos.Bookings;
using Booking.Contracts.Requests.Bookings;

namespace Booking.Application.Abstractions;

public interface IBookingService
{
    Task<IReadOnlyList<BookingDto>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<BookingDto> CreateAsync(CreateBookingRequest request, CancellationToken cancellationToken = default);
    Task<bool> CancelAsync(string bookingId, CancellationToken cancellationToken = default);
}