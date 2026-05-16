namespace Booking.Domain.Chats;

public class ChatMessage
{
    public string Id { get; set; } = default!;

    public string ThreadId { get; set; } = default!;
    public ChatThread Thread { get; set; } = default!;

    public string SenderUserId { get; set; } = default!;

    public string Text { get; set; } = default!;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public bool IsReadByGuest { get; set; }
    public bool IsReadByOwner { get; set; }
}