namespace Booking.Contracts.Requests.Admin;

public class ChangeUserRoleRequest
{
    public string Role { get; set; } = default!;
}