using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Users;

public class AppUser
{
    public string Id { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public bool Verified { get; set; }
    public string Phone { get; set; } = default!;
    public string Country { get; set; } = default!;
    public string PreferredCurrency { get; set; } = default!;
    public DateOnly Birthday { get; set; }
    public List<string> Favorites { get; set; } = [];
}