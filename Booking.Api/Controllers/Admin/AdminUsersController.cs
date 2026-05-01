using Booking.Application.Abstractions.Admin;
using Booking.Contracts.Requests.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public class AdminUsersController(IAdminUserService adminUserService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var users = await adminUserService.GetAllAsync(cancellationToken);
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        var user = await adminUserService.GetByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return NotFound(new { message = "User not found." });
        }

        return Ok(user);
    }

    [HttpPatch("{id}/block")]
    public async Task<IActionResult> Block(
        string id,
        CancellationToken cancellationToken)
    {
        var user = await adminUserService.BlockAsync(id, cancellationToken);

        if (user is null)
        {
            return NotFound(new { message = "User not found." });
        }

        return Ok(user);
    }

    [HttpPatch("{id}/unblock")]
    public async Task<IActionResult> Unblock(
        string id,
        CancellationToken cancellationToken)
    {
        var user = await adminUserService.UnblockAsync(id, cancellationToken);

        if (user is null)
        {
            return NotFound(new { message = "User not found." });
        }

        return Ok(user);
    }

    [HttpPatch("{id}/role")]
    public async Task<IActionResult> ChangeRole(
        string id,
        [FromBody] ChangeUserRoleRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await adminUserService.ChangeRoleAsync(id, request, cancellationToken);

            if (user is null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}