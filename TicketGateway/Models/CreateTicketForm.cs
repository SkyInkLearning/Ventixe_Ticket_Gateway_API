namespace TicketGateway.Models;

public class CreateTicketForm
{
    public string EventId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string InvoiceId { get; set; } = null!;
    public string TicketCategory { get; set; } = null!;
    public string SeatNumber { get; set; } = null!;
    public int Gate { get; set; }
}
