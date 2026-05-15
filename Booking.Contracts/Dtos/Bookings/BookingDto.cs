namespace Booking.Contracts.Dtos.Bookings;

public class BookingDto
{
    public string Id { get; set; } = default!;

    public string UserId { get; set; } = default!;
    public string? HotelOwnerUserId { get; set; }

    public string HotelId { get; set; } = default!;
    public string RoomId { get; set; } = default!;

    public string CheckIn { get; set; } = default!;
    public string CheckOut { get; set; } = default!;

    public int Guests { get; set; }

    public string Status { get; set; } = default!;

    public decimal TotalPrice { get; set; }
    public string Currency { get; set; } = default!;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? OwnerRespondedAtUtc { get; set; }
    public DateTime? CancelledAtUtc { get; set; }

    public string? CancellationReason { get; set; }
}