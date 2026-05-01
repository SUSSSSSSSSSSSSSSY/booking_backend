using Booking.Application.Abstractions;
using Booking.Contracts.Requests.Bookings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController(
    IBookingService bookingService,
    ICurrentUserService currentUser) : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyBookings(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        var bookings = await bookingService.GetByUserIdAsync(
            currentUser.UserId,
            cancellationToken);

        return Ok(bookings);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateBookingRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        try
        {
            var booking = await bookingService.CreateAsync(
                currentUser.UserId,
                request,
                cancellationToken);

            return Ok(booking);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPatch("{bookingId}/cancel")]
    public async Task<IActionResult> Cancel(
        string bookingId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        var result = await bookingService.CancelAsync(
            currentUser.UserId,
            bookingId,
            cancellationToken);

        if (!result)
        {
            return NotFound(new { message = "Booking not found." });
        }

        return NoContent();
    }

    // Временно можно оставить старый endpoint для фронта,
    // но новый фронт должен переходить на /api/bookings/me
    [HttpGet("user/{userId}")]
    [Obsolete("Use GET /api/bookings/me instead.")]
    public async Task<IActionResult> GetByUser(
        string userId,
        CancellationToken cancellationToken)
    {
        var bookings = await bookingService.GetByUserIdAsync(userId, cancellationToken);
        return Ok(bookings);
    }
}