using Booking.Application.Abstractions;
using Booking.Contracts.Common;
using Booking.Contracts.Dtos.Chats;
using Booking.Contracts.Requests.Chats;
using Booking.Domain.Chats;
using Booking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Services.Ef;

public class EfChatService(BookingDbContext dbContext) : IChatService
{
    public async Task<ChatThreadDto> GetOrCreateThreadForBookingAsync(
        string userId,
        string bookingId,
        CancellationToken cancellationToken = default)
    {
        var booking = await dbContext.Bookings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == bookingId, cancellationToken);

        if (booking is null)
        {
            throw new InvalidOperationException("Booking not found.");
        }

        if (booking.UserId != userId && booking.HotelOwnerUserId != userId)
        {
            throw new InvalidOperationException("You do not have access to this booking chat.");
        }

        if (string.IsNullOrWhiteSpace(booking.HotelOwnerUserId))
        {
            throw new InvalidOperationException("This booking does not have a hotel owner chat.");
        }

        var thread = await dbContext.ChatThreads
            .Include(x => x.Messages.OrderByDescending(m => m.CreatedAtUtc).Take(1))
            .FirstOrDefaultAsync(x => x.BookingId == bookingId, cancellationToken);

        if (thread is null)
        {
            thread = new ChatThread
            {
                Id = $"thr_{Guid.NewGuid():N}"[..16],
                BookingId = booking.Id,
                GuestUserId = booking.UserId,
                OwnerUserId = booking.HotelOwnerUserId,
                HotelId = booking.HotelId,
                RoomId = booking.RoomId,
                CreatedAtUtc = DateTime.UtcNow,
                IsClosed = false
            };

            dbContext.ChatThreads.Add(thread);

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return ToThreadDto(thread);
    }

    public async Task<PagedResult<ChatThreadDto>> GetMyThreadsAsync(
        string userId,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        pagination.Normalize();

        var query = dbContext.ChatThreads
            .AsNoTracking()
            .Include(x => x.Messages.OrderByDescending(m => m.CreatedAtUtc).Take(1))
            .Where(x => x.GuestUserId == userId || x.OwnerUserId == userId)
            .OrderByDescending(x => x.LastMessageAtUtc ?? x.CreatedAtUtc)
            .AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var threads = await query
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        var items = threads
            .Select(ToThreadDto)
            .ToList();

        return PagedResult<ChatThreadDto>.Create(
            items,
            pagination.Page,
            pagination.PageSize,
            totalCount);
    }

    public async Task<PagedResult<ChatMessageDto>> GetMessagesAsync(
        string userId,
        string threadId,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        pagination.Normalize();

        var thread = await dbContext.ChatThreads
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == threadId, cancellationToken);

        if (thread is null)
        {
            throw new InvalidOperationException("Chat thread not found.");
        }

        EnsureUserHasAccess(thread, userId);

        var query = dbContext.ChatMessages
            .AsNoTracking()
            .Where(x => x.ThreadId == threadId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var messages = await query
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        var items = messages
            .OrderBy(x => x.CreatedAtUtc)
            .Select(x => ToMessageDto(x, userId))
            .ToList();

        return PagedResult<ChatMessageDto>.Create(
            items,
            pagination.Page,
            pagination.PageSize,
            totalCount);
    }

    public async Task<ChatMessageDto> SendMessageAsync(
        string userId,
        string threadId,
        SendChatMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            throw new InvalidOperationException("Message text is required.");
        }

        if (request.Text.Length > 4000)
        {
            throw new InvalidOperationException("Message text is too long.");
        }

        var thread = await dbContext.ChatThreads
            .FirstOrDefaultAsync(x => x.Id == threadId, cancellationToken);

        if (thread is null)
        {
            throw new InvalidOperationException("Chat thread not found.");
        }

        EnsureUserHasAccess(thread, userId);

        if (thread.IsClosed)
        {
            throw new InvalidOperationException("Chat thread is closed.");
        }

        var message = new ChatMessage
        {
            Id = $"msg_{Guid.NewGuid():N}"[..16],
            ThreadId = thread.Id,
            SenderUserId = userId,
            Text = request.Text.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            IsReadByGuest = userId == thread.GuestUserId,
            IsReadByOwner = userId == thread.OwnerUserId
        };

        thread.LastMessageAtUtc = message.CreatedAtUtc;

        dbContext.ChatMessages.Add(message);

        await dbContext.SaveChangesAsync(cancellationToken);

        return ToMessageDto(message, userId);
    }

    public async Task<bool> MarkAsReadAsync(
        string userId,
        string threadId,
        CancellationToken cancellationToken = default)
    {
        var thread = await dbContext.ChatThreads
            .FirstOrDefaultAsync(x => x.Id == threadId, cancellationToken);

        if (thread is null)
        {
            return false;
        }

        EnsureUserHasAccess(thread, userId);

        var unreadMessages = await dbContext.ChatMessages
            .Where(x =>
                x.ThreadId == threadId &&
                x.SenderUserId != userId)
            .ToListAsync(cancellationToken);

        foreach (var message in unreadMessages)
        {
            if (userId == thread.GuestUserId)
            {
                message.IsReadByGuest = true;
            }

            if (userId == thread.OwnerUserId)
            {
                message.IsReadByOwner = true;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static void EnsureUserHasAccess(ChatThread thread, string userId)
    {
        if (thread.GuestUserId != userId && thread.OwnerUserId != userId)
        {
            throw new InvalidOperationException("You do not have access to this chat.");
        }
    }

    private static ChatThreadDto ToThreadDto(ChatThread thread)
    {
        var lastMessage = thread.Messages
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefault();

        return new ChatThreadDto
        {
            Id = thread.Id,
            BookingId = thread.BookingId,
            GuestUserId = thread.GuestUserId,
            OwnerUserId = thread.OwnerUserId,
            HotelId = thread.HotelId,
            RoomId = thread.RoomId,
            CreatedAtUtc = thread.CreatedAtUtc,
            LastMessageAtUtc = thread.LastMessageAtUtc,
            IsClosed = thread.IsClosed,
            LastMessageText = lastMessage?.Text
        };
    }

    private static ChatMessageDto ToMessageDto(ChatMessage message, string currentUserId)
    {
        return new ChatMessageDto
        {
            Id = message.Id,
            ThreadId = message.ThreadId,
            SenderUserId = message.SenderUserId,
            Text = message.Text,
            CreatedAtUtc = message.CreatedAtUtc,
            IsMine = message.SenderUserId == currentUserId
        };
    }

    public async Task<bool> UserHasAccessToThreadAsync(
    string userId,
    string threadId,
    CancellationToken cancellationToken = default)
    {
        return await dbContext.ChatThreads
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == threadId &&
                     (x.GuestUserId == userId || x.OwnerUserId == userId),
                cancellationToken);
    }
}