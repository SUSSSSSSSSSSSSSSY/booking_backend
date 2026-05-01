namespace Booking.Contracts.Dtos.Admin;

public class AdminUserDto
{
    public string Id { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;

    public string Role { get; set; } = default!;
    public bool IsBlocked { get; set; }
    public bool Verified { get; set; }

    public string Phone { get; set; } = "";
    public string Country { get; set; } = "";
    public string PreferredCurrency { get; set; } = "USD";
    public string Birthday { get; set; } = "";

    public int FavoritesCount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}