using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Storage;

public class RefreshSession
{
    public string UserId { get; set; } = default!;
    public DateTime ExpiresAtUtc { get; set; }
}