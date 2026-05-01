using Booking.Application.Abstractions.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/reviews")]
[Authorize(Roles = "Admin")]
public class AdminReviewsController(IAdminReviewService adminReviewService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? hotelId,
        CancellationToken cancellationToken)
    {
        var reviews = await adminReviewService.GetAllAsync(hotelId, cancellationToken);
        return Ok(reviews);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        var review = await adminReviewService.GetByIdAsync(id, cancellationToken);

        if (review is null)
        {
            return NotFound(new { message = "Review not found." });
        }

        return Ok(review);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        string id,
        CancellationToken cancellationToken)
    {
        var deleted = await adminReviewService.DeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound(new { message = "Review not found." });
        }

        return NoContent();
    }
}