namespace Booking.Contracts.Requests.Chats;

public class SendChatMessageRequest
{
    public string Text { get; set; } = default!;
}