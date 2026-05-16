namespace Booking.Domain.Chats;

public class ChatThread
{
    public string Id { get; set; } = default!;

    public string BookingId { get; set; } = default!;

    public string GuestUserId { get; set; } = default!;

    public string OwnerUserId { get; set; } = default!;

    public string HotelId { get; set; } = default!;
    public string RoomId { get; set; } = default!;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? LastMessageAtUtc { get; set; }

    public bool IsClosed { get; set; }

    public List<ChatMessage> Messages { get; set; } = [];
}