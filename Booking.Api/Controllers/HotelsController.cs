using Booking.Application.Abstractions;
using Booking.Contracts.Common;
using Booking.Contracts.Requests.Hotels;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelsController(IHotelService hotelService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken)
    {
        var hotels = await hotelService.GetAllAsync(pagination, cancellationToken);
        return Ok(hotels);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        var hotel = await hotelService.GetByIdAsync(id, cancellationToken);

        if (hotel is null)
        {
            return NotFound(new { message = "Hotel not found." });
        }

        return Ok(hotel);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] HotelSearchRequest request,
        CancellationToken cancellationToken)
    {
        var hotels = await hotelService.SearchAsync(request, cancellationToken);
        return Ok(hotels);
    }
}