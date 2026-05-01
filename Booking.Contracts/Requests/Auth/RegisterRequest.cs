namespace Booking.Contracts.Requests.Auth;

public class RegisterRequest
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;

    public string? Phone { get; set; }
    public string? Country { get; set; }
    public string PreferredCurrency { get; set; } = "USD";
    public string? Birthday { get; set; }
}