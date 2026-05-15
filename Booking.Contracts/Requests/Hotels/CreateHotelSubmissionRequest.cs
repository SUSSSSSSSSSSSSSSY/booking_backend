using Booking.Contracts.Dtos.Hotels;

namespace Booking.Contracts.Requests.Hotels;

public class CreateHotelSubmissionRequest
{
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
}