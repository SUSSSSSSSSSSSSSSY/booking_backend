namespace Booking.Domain.Bookings;

public class HotelBooking
{
    public string Id { get; set; } = default!;

    public string UserId { get; set; } = default!;

    public string? HotelOwnerUserId { get; set; }

    public string HotelId { get; set; } = default!;
    public string RoomId { get; set; } = default!;

    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }

    public int Guests { get; set; }

    public string Status { get; set; } = "pending_owner_approval";

    public decimal TotalPrice { get; set; }
    public string Currency { get; set; } = "USD";

    public bool IsHiddenForUser { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? OwnerRespondedAtUtc { get; set; }
    public DateTime? CancelledAtUtc { get; set; }

    public string? CancellationReason { get; set; }
}