namespace Booking.Contracts.Dtos.Chats;

public class ChatMessageDto
{
    public string Id { get; set; } = default!;

    public string ThreadId { get; set; } = default!;

    public string SenderUserId { get; set; } = default!;

    public string Text { get; set; } = default!;

    public DateTime CreatedAtUtc { get; set; }

    public bool IsMine { get; set; }
}