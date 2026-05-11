using Booking.Application.Abstractions.Admin;
using Booking.Contracts.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/bookings")]
[Authorize(Roles = "Admin")]
public class AdminBookingsController(IAdminBookingService adminBookingService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] PaginationRequest pagination,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var bookings = await adminBookingService.GetAllAsync(
            pagination,
            status,
            cancellationToken);

        return Ok(bookings);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        var booking = await adminBookingService.GetByIdAsync(id, cancellationToken);

        if (booking is null)
        {
            return NotFound(new { message = "Booking not found." });
        }

        return Ok(booking);
    }

    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> Cancel(
        string id,
        CancellationToken cancellationToken)
    {
        var booking = await adminBookingService.CancelAsync(id, cancellationToken);

        if (booking is null)
        {
            return NotFound(new { message = "Booking not found." });
        }

        return Ok(booking);
    }
}