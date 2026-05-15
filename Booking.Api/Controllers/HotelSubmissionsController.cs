using Booking.Application.Abstractions;
using Booking.Contracts.Common;
using Booking.Contracts.Requests.Hotels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
[Route("api/hotel-submissions")]
[Authorize]
public class HotelSubmissionsController(
    IHotelSubmissionService hotelSubmissionService,
    ICurrentUserService currentUser) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateHotelSubmissionRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        try
        {
            var submission = await hotelSubmissionService.CreateAsync(
                currentUser.UserId,
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = submission.Id },
                submission);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMine(
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        var submissions = await hotelSubmissionService.GetMySubmissionsAsync(
            currentUser.UserId,
            pagination,
            cancellationToken);

        return Ok(submissions);
    }

    [HttpGet("me/{id}")]
    public async Task<IActionResult> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        var submission = await hotelSubmissionService.GetMySubmissionByIdAsync(
            currentUser.UserId,
            id,
            cancellationToken);

        if (submission is null)
        {
            return NotFound(new { message = "Hotel submission not found." });
        }

        return Ok(submission);
    }

    [HttpGet("my-hotels")]
    public async Task<IActionResult> GetMyHotels(
    [FromQuery] PaginationRequest pagination,
    CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        var hotels = await hotelSubmissionService.GetMyHotelsAsync(
            currentUser.UserId,
            pagination,
            cancellationToken);

        return Ok(hotels);
    }

    [HttpPost("my-hotels/{hotelId}/update-request")]
    public async Task<IActionResult> SubmitUpdate(
    string hotelId,
    [FromBody] UpdateOwnedHotelSubmissionRequest request,
    CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        try
        {
            var submission = await hotelSubmissionService.SubmitUpdateAsync(
                currentUser.UserId,
                hotelId,
                request,
                cancellationToken);

            return Ok(submission);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("my-hotels/{hotelId}/delete-request")]
    public async Task<IActionResult> SubmitDelete(
    string hotelId,
    [FromBody] DeleteOwnedHotelSubmissionRequest request,
    CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        try
        {
            var submission = await hotelSubmissionService.SubmitDeleteAsync(
                currentUser.UserId,
                hotelId,
                request,
                cancellationToken);

            return Ok(submission);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


}