using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            PricePerNight = hotel.PricePerNight,
            Rating = hotel.Rating,
            ReviewCount = hotel.ReviewCount,
            DistanceToCenterKm = hotel.DistanceToCenterKm,
            Tags = [.. hotel.Tags],
            Amenities = [.. hotel.Amenities],
            Description = hotel.Description,
            Images = [.. hotel.Images],
            Rooms = hotel.Rooms.Select(x => x.ToDto()).ToList()
        };
    }

    public static RoomDto ToDto(this Room room)
    {
        return new RoomDto
        {
            Id = room.Id,
            Name = room.Name,
            Beds = room.Beds,
            Price = room.Price,
            FreeCancellation = room.FreeCancellation
        };
    }
}