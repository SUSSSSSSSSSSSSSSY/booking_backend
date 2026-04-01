using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Contracts.Requests.Reviews;

public class CreateReviewRequest
{
    public string Author { get; set; } = default!;
    public int Rating { get; set; }
    public string Text { get; set; } = default!;
}