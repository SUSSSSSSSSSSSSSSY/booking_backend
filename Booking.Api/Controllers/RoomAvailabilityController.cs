using Booking.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
[Route("api")]
public class RoomAvailabilityController(
    IRoomAvailabilityService roomAvailabilityService) : ControllerBase
{
    [HttpGet("rooms/{roomId}/availability")]
    public async Task<IActionResult> GetRoomAvailability(
        string roomId,
        [FromQuery] string? checkIn,
        [FromQuery] string? checkOut,
        CancellationToken cancellationToken)
    {
        try
        {
            var availability = await roomAvailabilityService.GetRoomAvailabilityAsync(
                roomId,
                checkIn,
                checkOut,
                cancellationToken);

            if (availability is null)
            {
                return NotFound(new { message = "Room not found." });
            }

            return Ok(availability);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("hotels/{hotelId}/rooms/availability")]
    public async Task<IActionResult> GetHotelRoomsAvailability(
        string hotelId,
        [FromQuery] string? checkIn,
        [FromQuery] string? checkOut,
        CancellationToken cancellationToken)
    {
        try
        {
            var availability = await roomAvailabilityService.GetHotelRoomsAvailabilityAsync(
                hotelId,
                checkIn,
                checkOut,
                cancellationToken);

            return Ok(availability);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}