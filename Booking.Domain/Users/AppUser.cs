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

    // Для email/password auth.
    // У Google-only пользователей может быть null.
    public string? PasswordHash { get; set; }

    public string Role { get; set; } = "User";
    public bool IsBlocked { get; set; }

    public bool Verified { get; set; }

    public string? Phone { get; set; }
    public string? Country { get; set; }
    public string PreferredCurrency { get; set; } = "USD";
    public DateOnly? Birthday { get; set; }

    // Google account data
    public string? GoogleSubjectId { get; set; }
    public string? PictureUrl { get; set; }

    public List<string> Favorites { get; set; } = [];

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}