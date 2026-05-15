using Booking.Application.Abstractions;
using Booking.Application.Abstractions.Admin;
using Booking.Contracts.Common;
using Booking.Contracts.Requests.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/hotel-submissions")]
[Authorize(Roles = "Admin")]
public class AdminHotelSubmissionsController(
    IAdminHotelSubmissionService adminHotelSubmissionService,
    ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] PaginationRequest pagination,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var submissions = await adminHotelSubmissionService.GetAllAsync(
            pagination,
            status,
            cancellationToken);

        return Ok(submissions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        var submission = await adminHotelSubmissionService.GetByIdAsync(
            id,
            cancellationToken);

        if (submission is null)
        {
            return NotFound(new { message = "Hotel submission not found." });
        }

        return Ok(submission);
    }

    [HttpPatch("{id}/approve")]
    public async Task<IActionResult> Approve(
        string id,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            return Unauthorized(new { message = "Admin user is not authenticated." });
        }

        try
        {
            var submission = await adminHotelSubmissionService.ApproveAsync(
                currentUser.UserId,
                id,
                cancellationToken);

            if (submission is null)
            {
                return NotFound(new { message = "Hotel submission not found." });
            }

            return Ok(submission);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/reject")]
    public async Task<IActionResult> Reject(
        string id,
        [FromBody] RejectHotelSubmissionRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            return Unauthorized(new { message = "Admin user is not authenticated." });
        }

        try
        {
            var submission = await adminHotelSubmissionService.RejectAsync(
                currentUser.UserId,
                id,
                request,
                cancellationToken);

            if (submission is null)
            {
                return NotFound(new { message = "Hotel submission not found." });
            }

            return Ok(submission);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}