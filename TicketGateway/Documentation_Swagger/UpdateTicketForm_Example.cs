using Swashbuckle.AspNetCore.Filters;
using TicketGateway.Models;

namespace TicketGateway.Documentation_Swagger;

public class UpdateTicketForm_Example : IExamplesProvider<UpdateTicketForm>
{
    public UpdateTicketForm GetExamples() => new()
    {
        EventId = "56f58514-7581-4b18-97f5-b6eb5ba7b9c9",
        InvoiceId = "1",
        Gate = 1,
        SeatNumber = "11B",
        TicketCategory = "Silver",
        UserId = "5",
        TicketId = 2
    };
}

