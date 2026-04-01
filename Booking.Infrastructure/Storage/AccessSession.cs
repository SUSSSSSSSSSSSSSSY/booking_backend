using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Storage;

public class AccessSession
{
    public string UserId { get; set; } = default!;
    public DateTime ExpiresAtUtc { get; set; }
    public string RefreshToken { get; set; } = default!;
}