using Booking.Contracts.Dtos.Hotels;
using Booking.Domain.Hotels;

namespace Booking.Infrastructure.Mappers;

public static class HotelMapper
{
    public static HotelDto ToDto(this Hotel hotel)
    {
        return new HotelDto
        {
            Id = hotel.Id,
            Name = hotel.Name,
            City = hotel.City,
            Country = hotel.Country,
            Address = hotel.Address,
            PricePerNight = hotel.PricePerNight,
            Rating = hotel.Rating,
            ReviewCount = hotel.ReviewCount,
            DistanceToCenterKm = hotel.DistanceToCenterKm,

            Tags = hotel.Tags?.ToList() ?? [],
            Amenities = hotel.Amenities?.ToList() ?? [],
            Description = hotel.Description,
            Images = hotel.Images?.ToList() ?? [],

            ScoreItems = hotel.ScoreItems?
                .Select(x => x.ToDto())
                .ToList() ?? [],

            Facilities = hotel.Facilities?
                .Select(x => x.ToDto())
                .ToList() ?? [],

            Rooms = hotel.Rooms?
                .Select(x => x.ToDto())
                .ToList() ?? []
        };
    }

    public static RoomDto ToDto(this Room room)
    {
        return new RoomDto
        {
            Id = room.Id,
            Image = room.Image,
            Name = room.Name,
            Beds = room.Beds,
            Price = room.Price,
            FreeCancellation = room.FreeCancellation
        };
    }

    public static ScoreItemDto ToDto(this ScoreItem scoreItem)
    {
        return new ScoreItemDto
        {
            Label = scoreItem.Label,
            Value = scoreItem.Value
        };
    }

    public static FacilityGroupDto ToDto(this FacilityGroup facilityGroup)
    {
        return new FacilityGroupDto
        {
            Title = facilityGroup.Title,
            Icon = facilityGroup.Icon,
            Items = facilityGroup.Items?.ToList() ?? []
        };
    }
}