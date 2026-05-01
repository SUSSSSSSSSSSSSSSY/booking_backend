using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Hotels;

public class Room
{
    public string Id { get; set; } = default!;
    public string HotelId { get; set; } = default!;

    public string? Image { get; set; }
    public string Name { get; set; } = default!;
    public string Beds { get; set; } = default!;
    public decimal Price { get; set; }
    public bool FreeCancellation { get; set; }
}