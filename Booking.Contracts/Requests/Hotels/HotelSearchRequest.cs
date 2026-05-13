using Booking.Contracts.Common;

namespace Booking.Contracts.Requests.Hotels;

public class HotelSearchRequest : PaginationRequest
{
    public string? Query { get; set; }

    public string? Name { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Address { get; set; }

    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    public double? MinRating { get; set; }
    public int? MinReviewCount { get; set; }

    public decimal? MaxDistanceToCenterKm { get; set; }

    public string? Tags { get; set; }
    public string? Amenities { get; set; }
    public string? Facilities { get; set; }

    public string? ScoreLabel { get; set; }
    public double? MinScoreValue { get; set; }

    public string? RoomName { get; set; }
    public string? RoomBeds { get; set; }
    public decimal? MinRoomPrice { get; set; }
    public decimal? MaxRoomPrice { get; set; }
    public bool? FreeCancellation { get; set; }

    public string? Sort { get; set; } = "recommended";

    public bool MatchAllTags { get; set; } = false;
    public bool MatchAllAmenities { get; set; } = false;
    public bool MatchAllFacilities { get; set; } = false;
}