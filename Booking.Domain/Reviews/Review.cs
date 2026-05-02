using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Reviews;

public class Review
{
    public string Id { get; set; } = default!;

    public string HotelId { get; set; } = default!;
    public string? UserId { get; set; } = default!;

    public string Author { get; set; } = default!;
    public int Rating { get; set; }
    public string Text { get; set; } = default!;

    public bool IsDeleted { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}