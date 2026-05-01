using Booking.Application.Abstractions.Admin;
using Booking.Contracts.Requests.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/hotels")]
[Authorize(Roles = "Admin")]
public class AdminHotelsController(IAdminHotelService adminHotelService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var hotels = await adminHotelService.GetAllAsync(cancellationToken);
        return Ok(hotels);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        var hotel = await adminHotelService.GetByIdAsync(id, cancellationToken);

        if (hotel is null)
        {
            return NotFound(new { message = "Hotel not found." });
        }

        return Ok(hotel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateHotelRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var hotel = await adminHotelService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = hotel.Id }, hotel);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdateHotelRequest request,
        CancellationToken cancellationToken)
    {
        var hotel = await adminHotelService.UpdateAsync(id, request, cancellationToken);

        if (hotel is null)
        {
            return NotFound(new { message = "Hotel not found." });
        }

        return Ok(hotel);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var deleted = await adminHotelService.DeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound(new { message = "Hotel not found." });
        }

        return NoContent();
    }

    [HttpPost("{hotelId}/rooms")]
    public async Task<IActionResult> AddRoom(
        string hotelId,
        [FromBody] CreateRoomRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var hotel = await adminHotelService.AddRoomAsync(hotelId, request, cancellationToken);

            if (hotel is null)
            {
                return NotFound(new { message = "Hotel not found." });
            }

            return Ok(hotel);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{hotelId}/rooms/{roomId}")]
    public async Task<IActionResult> UpdateRoom(
        string hotelId,
        string roomId,
        [FromBody] UpdateRoomRequest request,
        CancellationToken cancellationToken)
    {
        var hotel = await adminHotelService.UpdateRoomAsync(
            hotelId,
            roomId,
            request,
            cancellationToken);

        if (hotel is null)
        {
            return NotFound(new { message = "Hotel or room not found." });
        }

        return Ok(hotel);
    }

    [HttpDelete("{hotelId}/rooms/{roomId}")]
    public async Task<IActionResult> DeleteRoom(
        string hotelId,
        string roomId,
        CancellationToken cancellationToken)
    {
        var deleted = await adminHotelService.DeleteRoomAsync(
            hotelId,
            roomId,
            cancellationToken);

        if (!deleted)
        {
            return NotFound(new { message = "Hotel or room not found." });
        }

        return NoContent();
    }
}