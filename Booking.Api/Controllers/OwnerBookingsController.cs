using Booking.Application.Abstractions;
using Booking.Contracts.Common;
using Booking.Contracts.Requests.Bookings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
[Route("api/owner/bookings")]
[Authorize]
public class OwnerBookingsController(
    IOwnerBookingService ownerBookingService,
    ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] PaginationRequest pagination,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        var bookings = await ownerBookingService.GetAllForOwnerAsync(
            currentUser.UserId,
            pagination,
            status,
            cancellationToken);

        return Ok(bookings);
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending(
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        var bookings = await ownerBookingService.GetPendingForOwnerAsync(
            currentUser.UserId,
            pagination,
            cancellationToken);

        return Ok(bookings);
    }

    [HttpGet("{bookingId}")]
    public async Task<IActionResult> GetById(
        string bookingId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        var booking = await ownerBookingService.GetByIdForOwnerAsync(
            currentUser.UserId,
            bookingId,
            cancellationToken);

        if (booking is null)
        {
            return NotFound(new { message = "Booking not found." });
        }

        return Ok(booking);
    }

    [HttpPatch("{bookingId}/accept")]
    public async Task<IActionResult> Accept(
        string bookingId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        try
        {
            var booking = await ownerBookingService.AcceptAsync(
                currentUser.UserId,
                bookingId,
                cancellationToken);

            if (booking is null)
            {
                return NotFound(new { message = "Booking not found." });
            }

            return Ok(booking);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{bookingId}/reject")]
    public async Task<IActionResult> Reject(
        string bookingId,
        [FromBody] OwnerBookingDecisionRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        try
        {
            var booking = await ownerBookingService.RejectAsync(
                currentUser.UserId,
                bookingId,
                request.Reason,
                cancellationToken);

            if (booking is null)
            {
                return NotFound(new { message = "Booking not found." });
            }

            return Ok(booking);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{bookingId}/cancel")]
    public async Task<IActionResult> Cancel(
        string bookingId,
        [FromBody] OwnerBookingDecisionRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        var booking = await ownerBookingService.CancelAsync(
            currentUser.UserId,
            bookingId,
            request.Reason,
            cancellationToken);

        if (booking is null)
        {
            return NotFound(new { message = "Booking not found." });
        }

        return Ok(booking);
    }
}