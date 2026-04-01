using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Contracts.Dtos.Hotels;

public class RoomDto
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Beds { get; set; } = default!;
    public decimal Price { get; set; }
    public bool FreeCancellation { get; set; }
}