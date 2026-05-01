using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Contracts.Dtos.Hotels;

public class HotelDto
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string City { get; set; } = default!;
    public string Country { get; set; } = default!;
    public string? Address { get; set; }

    public decimal PricePerNight { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public decimal DistanceToCenterKm { get; set; }

    public List<string> Tags { get; set; } = [];
    public List<string> Amenities { get; set; } = [];
    public string Description { get; set; } = default!;
    public List<string> Images { get; set; } = [];

    public List<ScoreItemDto> ScoreItems { get; set; } = [];
    public List<FacilityGroupDto> Facilities { get; set; } = [];
    public List<RoomDto> Rooms { get; set; } = [];
}