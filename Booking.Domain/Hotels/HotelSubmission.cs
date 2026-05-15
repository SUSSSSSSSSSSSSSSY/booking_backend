namespace Booking.Domain.Hotels;

public class HotelSubmission
{
    public string Id { get; set; } = default!;

    public string SubmittedByUserId { get; set; } = default!;

    public string SubmissionType { get; set; } = "create";

    public string? TargetHotelId { get; set; }
    public string? ApprovedHotelId { get; set; }

    public string Status { get; set; } = "pending";

    public string Name { get; set; } = default!;
    public string City { get; set; } = default!;
    public string Country { get; set; } = default!;
    public string? Address { get; set; }

    public decimal PricePerNight { get; set; }
    public decimal DistanceToCenterKm { get; set; }

    public List<string> Tags { get; set; } = [];
    public List<string> Amenities { get; set; } = [];

    public string Description { get; set; } = default!;
    public List<string> Images { get; set; } = [];

    public List<ScoreItem> ScoreItems { get; set; } = [];
    public List<FacilityGroup> Facilities { get; set; } = [];
    public List<Room> Rooms { get; set; } = [];

    public string? AdminComment { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAtUtc { get; set; }
    public string? ReviewedByAdminId { get; set; }
}