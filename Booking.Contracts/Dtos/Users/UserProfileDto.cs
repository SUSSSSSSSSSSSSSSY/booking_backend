using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Contracts.Dtos.Users;

public class UserProfileDto
{
    public string Id { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool Verified { get; set; }
    public string Phone { get; set; } = default!;
    public string Country { get; set; } = default!;
    public string PreferredCurrency { get; set; } = default!;
    public string Birthday { get; set; } = default!;
    public List<string> Favorites { get; set; } = [];
}