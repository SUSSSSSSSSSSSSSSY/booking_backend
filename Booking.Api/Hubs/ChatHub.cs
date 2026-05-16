using Booking.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Booking.Api.Hubs;

[Authorize]
public class ChatHub(
    ICurrentUserService currentUser,
    IChatService chatService) : Hub
{
    public async Task JoinThread(string threadId)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            throw new HubException("User is not authenticated.");
        }

        var hasAccess = await chatService.UserHasAccessToThreadAsync(
            currentUser.UserId,
            threadId,
            Context.ConnectionAborted);

        if (!hasAccess)
        {
            throw new HubException("You do not have access to this chat.");
        }

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            GetThreadGroupName(threadId));
    }

    public async Task LeaveThread(string threadId)
    {
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            GetThreadGroupName(threadId));
    }

    public static string GetThreadGroupName(string threadId)
    {
        return $"chat-thread-{threadId}";
    }
}