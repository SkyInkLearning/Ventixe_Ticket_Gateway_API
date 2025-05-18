namespace TicketGateway.Models;

public class TicketUserEventSeatKey
{
    public string UserId { get; set; } = null!;
    public string EventId { get; set; } = null!;
    public string SeatNumber { get; set; } = null!;
}
