using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Hotels;

public class ScoreItem
{
    public string Label { get; set; } = default!;
    public double Value { get; set; }
}