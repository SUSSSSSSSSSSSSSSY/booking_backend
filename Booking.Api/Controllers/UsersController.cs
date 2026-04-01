using Booking.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetById(string userId, CancellationToken cancellationToken)
    {
        var user = await userService.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet("{userId}/favorites")]
    public async Task<IActionResult> GetFavorites(string userId, CancellationToken cancellationToken)
    {
        var favorites = await userService.GetFavoritesAsync(userId, cancellationToken);
        return Ok(favorites);
    }

    [HttpPost("{userId}/favorites/{hotelId}")]
    public async Task<IActionResult> AddFavorite(string userId, string hotelId, CancellationToken cancellationToken)
    {
        var ok = await userService.AddFavoriteAsync(userId, hotelId, cancellationToken);
        if (!ok)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{userId}/favorites/{hotelId}")]
    public async Task<IActionResult> RemoveFavorite(string userId, string hotelId, CancellationToken cancellationToken)
    {
        var ok = await userService.RemoveFavoriteAsync(userId, hotelId, cancellationToken);
        if (!ok)
        {
            return NotFound();
        }

        return NoContent();
    }
}