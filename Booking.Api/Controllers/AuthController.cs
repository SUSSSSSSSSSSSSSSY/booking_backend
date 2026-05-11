using Booking.Application.Abstractions;
using Booking.Contracts.Requests.Auth;
using Booking.Contracts.Responses.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await authService.RegisterAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request, cancellationToken);

        if (result.Succeeded && result.Data is not null)
        {
            return Ok(result.Data);
        }

        return result.ErrorCode switch
        {
            AuthErrorCode.UserBlocked => StatusCode(StatusCodes.Status403Forbidden, new
            {
                code = "USER_BLOCKED",
                message = "This account has been blocked."
            }),

            _ => Unauthorized(new
            {
                code = "INVALID_CREDENTIALS",
                message = "Invalid email or password."
            })
        };
    }

    [HttpPost("google")]
    public async Task<IActionResult> Google(
        [FromBody] GoogleLoginRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await authService.LoginWithGoogleAsync(request, cancellationToken);
            return Ok(result);
        }
        catch
        {
            return Unauthorized(new { message = "Invalid Google token." });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result = await authService.RefreshAsync(request, cancellationToken);

        if (result is null)
        {
            return Unauthorized(new { message = "Invalid refresh token." });
        }

        return Ok(result);
    }
}