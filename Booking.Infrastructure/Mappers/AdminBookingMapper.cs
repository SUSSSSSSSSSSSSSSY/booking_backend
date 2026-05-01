using Booking.Contracts.Dtos.Admin;
using Booking.Domain.Bookings;
using Booking.Domain.Hotels;
using Booking.Domain.Users;

namespace Booking.Infrastructure.Mappers;

public static class AdminBookingMapper
{
    public static AdminBookingDto ToAdminDto(
        this HotelBooking booking,
        AppUser? user,
        Hotel? hotel,
        Room? room)
    {
        return new AdminBookingDto
        {
            Id = booking.Id,

            UserId = booking.UserId,
            UserEmail = user?.Email,
            UserFullName = user?.FullName,

            HotelId = booking.HotelId,
            HotelName = hotel?.Name,

            RoomId = booking.RoomId,
            RoomName = room?.Name,

            CheckIn = booking.CheckIn.ToString("yyyy-MM-dd"),
            CheckOut = booking.CheckOut.ToString("yyyy-MM-dd"),

            Guests = booking.Guests,

            Status = booking.Status,
            TotalPrice = booking.TotalPrice,
            Currency = booking.Currency,

            CreatedAtUtc = booking.CreatedAtUtc
        };
    }
}