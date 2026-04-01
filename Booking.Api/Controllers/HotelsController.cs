using Booking.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelsController(IHotelService hotelService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? city, [FromQuery] string? country, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(city) || !string.IsNullOrWhiteSpace(country))
        {
            var filtered = await hotelService.SearchAsync(city, country, cancellationToken);
            return Ok(filtered);
        }

        var hotels = await hotelService.GetAllAsync(cancellationToken);
        return Ok(hotels);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        var hotel = await hotelService.GetByIdAsync(id, cancellationToken);
        if (hotel is null)
        {
            return NotFound();
        }

        return Ok(hotel);
    }
}