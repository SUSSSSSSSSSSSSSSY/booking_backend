using Booking.Application.Abstractions;
using Booking.Contracts.Common;
using Booking.Contracts.Requests.Chats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Booking.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Booking.Api.Controllers;

[ApiController]
[Route("api/chats")]
[Authorize]
public class ChatsController(
    IChatService chatService,
    ICurrentUserService currentUser,
    IHubContext<ChatHub> chatHub) : ControllerBase
{
    [HttpPost("booking/{bookingId}")]
    public async Task<IActionResult> GetOrCreateForBooking(
        string bookingId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        try
        {
            var thread = await chatService.GetOrCreateThreadForBookingAsync(
                currentUser.UserId,
                bookingId,
                cancellationToken);

            return Ok(thread);
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

        var threads = await chatService.GetMyThreadsAsync(
            currentUser.UserId,
            pagination,
            cancellationToken);

        return Ok(threads);
    }

    [HttpGet("{threadId}/messages")]
    public async Task<IActionResult> GetMessages(
        string threadId,
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        try
        {
            var messages = await chatService.GetMessagesAsync(
                currentUser.UserId,
                threadId,
                pagination,
                cancellationToken);

            return Ok(messages);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{threadId}/messages")]
    public async Task<IActionResult> SendMessage(
    string threadId,
    [FromBody] SendChatMessageRequest request,
    CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        try
        {
            var message = await chatService.SendMessageAsync(
                currentUser.UserId,
                threadId,
                request,
                cancellationToken);

            await chatHub
                .Clients
                .Group(ChatHub.GetThreadGroupName(threadId))
                .SendAsync("messageReceived", message, cancellationToken);

            return Ok(message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{threadId}/read")]
    public async Task<IActionResult> MarkAsRead(
        string threadId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        try
        {
            var success = await chatService.MarkAsReadAsync(
                currentUser.UserId,
                threadId,
                cancellationToken);

            if (!success)
            {
                return NotFound(new { message = "Chat thread not found." });
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}