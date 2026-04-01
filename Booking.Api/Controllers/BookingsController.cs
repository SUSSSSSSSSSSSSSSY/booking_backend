using Booking.Application.Abstractions;
using Booking.Contracts.Requests.Bookings;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController(IBookingService bookingService) : ControllerBase
{
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(string userId, CancellationToken cancellationToken)
    {
        var bookings = await bookingService.GetByUserIdAsync(userId, cancellationToken);
        return Ok(bookings);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookingRequest request, CancellationToken cancellationToken)
    {
        var booking = await bookingService.CreateAsync(request, cancellationToken);
        return Ok(booking);
    }

    [HttpPatch("{bookingId}/cancel")]
    public async Task<IActionResult> Cancel(string bookingId, CancellationToken cancellationToken)
    {
        var ok = await bookingService.CancelAsync(bookingId, cancellationToken);
        if (!ok)
        {
            return NotFound();
        }

        return NoContent();
    }
}