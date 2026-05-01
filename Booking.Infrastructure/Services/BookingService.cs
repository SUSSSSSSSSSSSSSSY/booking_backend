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
    public Task<IReadOnlyList<BookingDto>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var result = store.Bookings
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CheckIn)
            .Select(x => x.ToDto())
            .ToList();

        return Task.FromResult<IReadOnlyList<BookingDto>>(result);
    }

    public Task<BookingDto> CreateAsync(
        string userId,
        CreateBookingRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new InvalidOperationException("User id is required.");
        }

        var checkIn = DateOnly.Parse(request.CheckIn);
        var checkOut = DateOnly.Parse(request.CheckOut);

        if (checkOut <= checkIn)
        {
            throw new InvalidOperationException("Check-out date must be after check-in date.");
        }

        if (request.Guests <= 0)
        {
            throw new InvalidOperationException("Guests count must be greater than zero.");
        }

        var hotel = store.Hotels.FirstOrDefault(x => x.Id == request.HotelId);

        if (hotel is null)
        {
            throw new InvalidOperationException("Hotel not found.");
        }

        var room = hotel.Rooms.FirstOrDefault(x => x.Id == request.RoomId);

        if (room is null)
        {
            throw new InvalidOperationException("Room not found.");
        }

        var nights = checkOut.DayNumber - checkIn.DayNumber;
        var totalPrice = room.Price * nights;

        var booking = new HotelBooking
        {
            Id = $"bok_{Guid.NewGuid():N}"[..12],
            UserId = userId,
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

    public Task<bool> CancelAsync(
        string userId,
        string bookingId,
        CancellationToken cancellationToken = default)
    {
        var booking = store.Bookings.FirstOrDefault(x =>
            x.Id == bookingId &&
            x.UserId == userId);

        if (booking is null)
        {
            return Task.FromResult(false);
        }

        if (booking.Status == "cancelled")
        {
            return Task.FromResult(true);
        }

        booking.Status = "cancelled";
        return Task.FromResult(true);
    }
}