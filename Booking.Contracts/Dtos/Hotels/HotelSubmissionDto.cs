namespace Booking.Contracts.Dtos.Hotels;

public class HotelSubmissionDto
{
    public string Id { get; set; } = default!;

    public string SubmittedByUserId { get; set; } = default!;

    public string SubmissionType { get; set; } = default!;
    public string? TargetHotelId { get; set; }
    public string? ApprovedHotelId { get; set; }

    public string Status { get; set; } = default!;

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

    public List<ScoreItemDto> ScoreItems { get; set; } = [];
    public List<FacilityGroupDto> Facilities { get; set; } = [];
    public List<RoomDto> Rooms { get; set; } = [];

    public string? AdminComment { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
    public string? ReviewedByAdminId { get; set; }
}