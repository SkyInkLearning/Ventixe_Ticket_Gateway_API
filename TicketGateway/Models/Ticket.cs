namespace TicketGateway.Models;

public class Ticket
{
    public int TicketId { get; set; }
    public string EventId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string InvoiceId { get; set; } = null!;
    public string TicketCategory { get; set; } = null!;
    public string SeatNumber { get; set; } = null!;
    public int Gate { get; set; }
}
