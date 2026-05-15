namespace Booking.Contracts.Dtos.Bookings;

public class RoomAvailabilityDto
{
    public string RoomId { get; set; } = default!;
    public string RoomName { get; set; } = default!;

    public bool IsAvailable { get; set; }

    public string? CheckedFrom { get; set; }
    public string? CheckedTo { get; set; }

    public List<BookedPeriodDto> BookedPeriods { get; set; } = [];
}