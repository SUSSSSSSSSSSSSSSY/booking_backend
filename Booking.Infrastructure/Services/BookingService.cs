using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Application.Abstractions;
using Booking.Contracts.Dtos.Bookings;
using Booking.Contracts.Requests.Bookings;
using Booking.Domain.Bookings;
using Booking.Infrastructure.Mappers;
using Booking.Infrastructure.Storage;

namespace Booking.Infrastructure.Services;

public class BookingService(InMemoryStore store) : IBookingService
{
    public Task<IReadOnlyList<BookingDto>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var result = store.Bookings
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CheckIn)
            .Select(x => x.ToDto())
            .ToList();

        return Task.FromResult<IReadOnlyList<BookingDto>>(result);
    }

    public Task<BookingDto> CreateAsync(CreateBookingRequest request, CancellationToken cancellationToken = default)
    {
        var checkIn = DateOnly.Parse(request.CheckIn);
        var checkOut = DateOnly.Parse(request.CheckOut);

        var hotel = store.Hotels.First(x => x.Id == request.HotelId);
        var room = hotel.Rooms.First(x => x.Id == request.RoomId);

        var nights = checkOut.DayNumber - checkIn.DayNumber;
        var totalPrice = room.Price * nights;

        var booking = new HotelBooking
        {
            Id = $"bok_{Guid.NewGuid():N}"[..12],
            UserId = request.UserId,
            HotelId = request.HotelId,
            RoomId = request.RoomId,
            CheckIn = checkIn,
            CheckOut = checkOut,
            Guests = request.Guests,
            Status = "confirmed",
            TotalPrice = totalPrice,
            Currency = request.Currency
        };

        store.Bookings.Add(booking);

        return Task.FromResult(booking.ToDto());
    }

    public Task<bool> CancelAsync(string bookingId, CancellationToken cancellationToken = default)
    {
        var booking = store.Bookings.FirstOrDefault(x => x.Id == bookingId);
        if (booking is null)
        {
            return Task.FromResult(false);
        }

        booking.Status = "cancelled";
        return Task.FromResult(true);
    }
}