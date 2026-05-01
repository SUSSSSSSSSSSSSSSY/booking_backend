namespace Booking.Contracts.Requests.Admin;

public class UpdateRoomRequest
{
    public string? Image { get; set; }
    public string Name { get; set; } = default!;
    public string Beds { get; set; } = default!;
    public decimal Price { get; set; }
    public bool FreeCancellation { get; set; }
}