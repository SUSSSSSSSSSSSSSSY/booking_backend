namespace Booking.Contracts.Dtos.Admin;

public class AdminReviewDto
{
    public string Id { get; set; } = default!;

    public string HotelId { get; set; } = default!;
    public string? HotelName { get; set; }

    public string Author { get; set; } = default!;
    public int Rating { get; set; }
    public string Text { get; set; } = default!;

    public DateTime CreatedAtUtc { get; set; }
    public int DaysAgo { get; set; }
}