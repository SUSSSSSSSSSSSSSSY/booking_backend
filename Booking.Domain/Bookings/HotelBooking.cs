using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Bookings;

public class HotelBooking
{
    public string Id { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public string HotelId { get; set; } = default!;
    public string RoomId { get; set; } = default!;
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public int Guests { get; set; }
    public string Status { get; set; } = default!;
    public decimal TotalPrice { get; set; }
    public string Currency { get; set; } = default!;
}