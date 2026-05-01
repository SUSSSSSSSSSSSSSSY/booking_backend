using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Contracts.Requests.Bookings;

public class CreateBookingRequest
{
    public string HotelId { get; set; } = default!;
    public string RoomId { get; set; } = default!;
    public string CheckIn { get; set; } = default!;
    public string CheckOut { get; set; } = default!;
    public int Guests { get; set; }
    public string Currency { get; set; } = "USD";
}