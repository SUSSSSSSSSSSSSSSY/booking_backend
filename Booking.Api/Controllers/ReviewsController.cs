using Booking.Application.Abstractions;
using Booking.Contracts.Requests.Reviews;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
[Route("api/hotels/{hotelId}/reviews")]
public class ReviewsController(IReviewService reviewService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetByHotel(string hotelId, CancellationToken cancellationToken)
    {
        var reviews = await reviewService.GetByHotelIdAsync(hotelId, cancellationToken);
        return Ok(reviews);
    }

    [HttpPost]
    public async Task<IActionResult> Create(string hotelId, [FromBody] CreateReviewRequest request, CancellationToken cancellationToken)
    {
        var review = await reviewService.CreateAsync(hotelId, request, cancellationToken);
        return Ok(review);
    }
}