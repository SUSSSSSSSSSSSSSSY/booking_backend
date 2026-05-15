using Booking.Application.Abstractions;
using Booking.Contracts.Dtos.Bookings;
using Booking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Services.Ef;

public class EfRoomAvailabilityService(BookingDbContext dbContext) : IRoomAvailabilityService
{
    private static readonly string[] BlockingStatuses =
    [
        "pending_owner_approval",
        "confirmed"
    ];

    public async Task<RoomAvailabilityDto?> GetRoomAvailabilityAsync(
        string roomId,
        string? checkIn,
        string? checkOut,
        CancellationToken cancellationToken = default)
    {
        var room = await dbContext.Rooms
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == roomId && !x.IsDeleted,
                cancellationToken);

        if (room is null)
        {
            return null;
        }

        var checkedRange = ParseOptionalRange(checkIn, checkOut);

        var bookedPeriods = await GetBookedPeriodsAsync(
            room.Id,
            cancellationToken);

        var isAvailable = checkedRange is null ||
                          !bookedPeriods.Any(x =>
                              DatesOverlap(
                                  DateOnly.Parse(x.CheckIn),
                                  DateOnly.Parse(x.CheckOut),
                                  checkedRange.Value.CheckIn,
                                  checkedRange.Value.CheckOut));

        return new RoomAvailabilityDto
        {
            RoomId = room.Id,
            RoomName = room.Name,
            IsAvailable = isAvailable,
            CheckedFrom = checkedRange?.CheckIn.ToString("yyyy-MM-dd"),
            CheckedTo = checkedRange?.CheckOut.ToString("yyyy-MM-dd"),
            BookedPeriods = bookedPeriods
        };
    }

    public async Task<IReadOnlyList<RoomAvailabilityDto>> GetHotelRoomsAvailabilityAsync(
        string hotelId,
        string? checkIn,
        string? checkOut,
        CancellationToken cancellationToken = default)
    {
        var hotel = await dbContext.Hotels
            .AsNoTracking()
            .Include(x => x.Rooms.Where(r => !r.IsDeleted))
            .FirstOrDefaultAsync(
                x => x.Id == hotelId && !x.IsDeleted,
                cancellationToken);

        if (hotel is null)
        {
            return [];
        }

        var checkedRange = ParseOptionalRange(checkIn, checkOut);

        var roomIds = hotel.Rooms
            .Select(x => x.Id)
            .ToList();

        var bookings = await dbContext.Bookings
            .AsNoTracking()
            .Where(x =>
                roomIds.Contains(x.RoomId) &&
                BlockingStatuses.Contains(x.Status))
            .OrderBy(x => x.CheckIn)
            .ToListAsync(cancellationToken);

        var result = hotel.Rooms
            .Select(room =>
            {
                var roomBookedPeriods = bookings
                    .Where(x => x.RoomId == room.Id)
                    .Select(x => new BookedPeriodDto
                    {
                        BookingId = x.Id,
                        CheckIn = x.CheckIn.ToString("yyyy-MM-dd"),
                        CheckOut = x.CheckOut.ToString("yyyy-MM-dd"),
                        Status = x.Status
                    })
                    .ToList();

                var isAvailable = checkedRange is null ||
                                  !roomBookedPeriods.Any(x =>
                                      DatesOverlap(
                                          DateOnly.Parse(x.CheckIn),
                                          DateOnly.Parse(x.CheckOut),
                                          checkedRange.Value.CheckIn,
                                          checkedRange.Value.CheckOut));

                return new RoomAvailabilityDto
                {
                    RoomId = room.Id,
                    RoomName = room.Name,
                    IsAvailable = isAvailable,
                    CheckedFrom = checkedRange?.CheckIn.ToString("yyyy-MM-dd"),
                    CheckedTo = checkedRange?.CheckOut.ToString("yyyy-MM-dd"),
                    BookedPeriods = roomBookedPeriods
                };
            })
            .ToList();

        return result;
    }

    private async Task<List<BookedPeriodDto>> GetBookedPeriodsAsync(
        string roomId,
        CancellationToken cancellationToken)
    {
        var bookings = await dbContext.Bookings
            .AsNoTracking()
            .Where(x =>
                x.RoomId == roomId &&
                BlockingStatuses.Contains(x.Status))
            .OrderBy(x => x.CheckIn)
            .ToListAsync(cancellationToken);

        return bookings
            .Select(x => new BookedPeriodDto
            {
                BookingId = x.Id,
                CheckIn = x.CheckIn.ToString("yyyy-MM-dd"),
                CheckOut = x.CheckOut.ToString("yyyy-MM-dd"),
                Status = x.Status
            })
            .ToList();
    }

    private static (DateOnly CheckIn, DateOnly CheckOut)? ParseOptionalRange(
        string? checkIn,
        string? checkOut)
    {
        if (string.IsNullOrWhiteSpace(checkIn) &&
            string.IsNullOrWhiteSpace(checkOut))
        {
            return null;
        }

        if (!DateOnly.TryParse(checkIn, out var parsedCheckIn))
        {
            throw new InvalidOperationException("Invalid check-in date.");
        }

        if (!DateOnly.TryParse(checkOut, out var parsedCheckOut))
        {
            throw new InvalidOperationException("Invalid check-out date.");
        }

        if (parsedCheckOut <= parsedCheckIn)
        {
            throw new InvalidOperationException("Check-out date must be after check-in date.");
        }

        return (parsedCheckIn, parsedCheckOut);
    }

    private static bool DatesOverlap(
        DateOnly firstCheckIn,
        DateOnly firstCheckOut,
        DateOnly secondCheckIn,
        DateOnly secondCheckOut)
    {
        return firstCheckIn < secondCheckOut &&
               secondCheckIn < firstCheckOut;
    }
}