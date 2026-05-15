namespace Booking.Contracts.Dtos.Bookings;

public class BookedPeriodDto
{
    public string BookingId { get; set; } = default!;

    public string CheckIn { get; set; } = default!;
    public string CheckOut { get; set; } = default!;

    public string Status { get; set; } = default!;
}