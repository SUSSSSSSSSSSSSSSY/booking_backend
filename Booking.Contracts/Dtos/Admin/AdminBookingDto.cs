namespace Booking.Contracts.Dtos.Admin;

public class AdminBookingDto
{
    public string Id { get; set; } = default!;

    public string UserId { get; set; } = default!;
    public string? UserEmail { get; set; }
    public string? UserFullName { get; set; }

    public string HotelId { get; set; } = default!;
    public string? HotelName { get; set; }

    public string RoomId { get; set; } = default!;
    public string? RoomName { get; set; }

    public string CheckIn { get; set; } = default!;
    public string CheckOut { get; set; } = default!;

    public int Guests { get; set; }

    public string Status { get; set; } = default!;
    public decimal TotalPrice { get; set; }
    public string Currency { get; set; } = default!;

    public DateTime CreatedAtUtc { get; set; }
}