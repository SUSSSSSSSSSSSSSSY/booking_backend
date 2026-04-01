using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Contracts.Dtos.Bookings;
using Booking.Domain.Bookings;

namespace Booking.Infrastructure.Mappers;

public static class BookingMapper
{
    public static BookingDto ToDto(this HotelBooking booking)
    {
        return new BookingDto
        {
            Id = booking.Id,
            UserId = booking.UserId,
            HotelId = booking.HotelId,
            RoomId = booking.RoomId,
            CheckIn = booking.CheckIn.ToString("yyyy-MM-dd"),
            CheckOut = booking.CheckOut.ToString("yyyy-MM-dd"),
            Guests = booking.Guests,
            Status = booking.Status,
            TotalPrice = booking.TotalPrice,
            Currency = booking.Currency
        };
    }
}