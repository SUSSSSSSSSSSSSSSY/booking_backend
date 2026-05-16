using Booking.Contracts.Common;
using Booking.Contracts.Dtos.Chats;
using Booking.Contracts.Requests.Chats;

namespace Booking.Application.Abstractions;

public interface IChatService
{
    Task<ChatThreadDto> GetOrCreateThreadForBookingAsync(
        string userId,
        string bookingId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<ChatThreadDto>> GetMyThreadsAsync(
        string userId,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default);

    Task<PagedResult<ChatMessageDto>> GetMessagesAsync(
        string userId,
        string threadId,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default);

    Task<ChatMessageDto> SendMessageAsync(
        string userId,
        string threadId,
        SendChatMessageRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> MarkAsReadAsync(
        string userId,
        string threadId,
        CancellationToken cancellationToken = default);

    Task<bool> UserHasAccessToThreadAsync(
    string userId,
    string threadId,
    CancellationToken cancellationToken = default);
}