using Booking.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(
    IUserService userService,
    ICurrentUserService currentUser) : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        var user = await userService.GetByIdAsync(currentUser.UserId, cancellationToken);

        if (user is null)
        {
            return NotFound(new { message = "User not found." });
        }

        return Ok(user);
    }

    [Authorize]
    [HttpGet("me/favorites")]
    public async Task<IActionResult> GetMyFavorites(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        var favorites = await userService.GetFavoritesAsync(currentUser.UserId, cancellationToken);
        return Ok(favorites);
    }

    [Authorize]
    [HttpPost("me/favorites/{hotelId}")]
    public async Task<IActionResult> AddMyFavorite(
        string hotelId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        var result = await userService.AddFavoriteAsync(
            currentUser.UserId,
            hotelId,
            cancellationToken);

        if (!result)
        {
            return NotFound(new { message = "User or hotel not found." });
        }

        return NoContent();
    }

    [Authorize]
    [HttpDelete("me/favorites/{hotelId}")]
    public async Task<IActionResult> RemoveMyFavorite(
        string hotelId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        var result = await userService.RemoveFavoriteAsync(
            currentUser.UserId,
            hotelId,
            cancellationToken);

        if (!result)
        {
            return NotFound(new { message = "User or hotel not found." });
        }

        return NoContent();
    }

    // временно можешь оставить старый endpoint для совместимости,
    // но лучше пометить как obsolete
    [HttpGet("{userId}")]
    [Obsolete("Use GET /api/users/me instead.")]
    public async Task<IActionResult> GetById(
        string userId,
        CancellationToken cancellationToken)
    {
        var user = await userService.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return NotFound(new { message = "User not found." });
        }

        return Ok(user);
    }
}