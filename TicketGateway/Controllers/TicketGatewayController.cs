using ExternalValidation.ApiSettings;
using ExternalValidation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using TicketGateway.Documentation_Swagger;
using TicketGateway.Extensions.Attributes;
using TicketGateway.Models;
using TicketGateway.Services;

namespace TicketGateway.Controllers;

[UseApiKey]
[Consumes("application/json")]
[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class TicketGatewayController(TicketSBSender sender, ExternalEventCheck eventCheck, ExternalUserCheck userCheck, ExternalInvoiceCheck invoiceCheck, HttpClient httpClient, IOptions<TicketServiceApiSettings> ticketServiceSettings) : ControllerBase
{
    private readonly TicketSBSender _sender = sender;

    private readonly ExternalEventCheck _eventCheck = eventCheck;
    private readonly ExternalUserCheck _userCheck = userCheck;
    private readonly ExternalInvoiceCheck _invoiceCheck = invoiceCheck;
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _ticketServiceUrl = ticketServiceSettings.Value.Url;


    //POST
    [HttpPost]
    [SwaggerOperation(Summary = "Creates a ticket if all data given is valid.")]
    [SwaggerResponse(200, "Sent the ticket to the service bus for creation.")]
    [SwaggerResponse(400, "The CreateTicketForm was either containing invalid or missing properties.")]
    [SwaggerRequestExample(typeof(CreateTicketForm), typeof(CreateTicketForm_Example))]
    public async Task<IActionResult> CreateTicket(CreateTicketForm createForm)
    {
        // Takes in a form checks the data against external services and then sends it to the SB sender.
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // External checks:
        var eventCheckResult = await _eventCheck.EventExistanceCheck(createForm.EventId);
        if (!eventCheckResult.Success) return BadRequest("No event with this id exists.");

        //var userCheckResult = await _userCheck.UserExistanceCheck(createForm.UserId);
        //if (!userCheckResult.Success) return BadRequest("No user with this id exists.");

        //var invoiceCheckResult = await _invoiceCheck.InvoiceExistanceCheck(createForm.InvoiceId);
        //if (!invoiceCheckResult.Success) return BadRequest("No invoice with this id exists.");

        var resultOfSender = await _sender.SendCreateAsync(createForm);
        if (!resultOfSender) { return BadRequest("Could not send the ticketcreation request."); }

        return Ok("Ticketcreation request sent.");
    }

    //PUT
    [HttpPut]
    [SwaggerOperation(Summary = "Updates a ticket if all data given is valid and the ticket exists.")]
    [SwaggerResponse(200, "Sent the update request of the ticket to the service bus for update.")]
    [SwaggerResponse(400, "The UpdateTicketForm was either containing invalid or missing properties.")]
    [SwaggerRequestExample(typeof(UpdateTicketForm), typeof(UpdateTicketForm_Example))]
    public async Task<IActionResult> UpdateTicket(UpdateTicketForm updateForm)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // External checks:
        var eventCheckResult = await _eventCheck.EventExistanceCheck(updateForm.EventId);
        if (!eventCheckResult.Success) return BadRequest("No event with this id exists.");

        //var userCheckResult = await _userCheck.UserExistanceCheck(updateForm.UserId);
        //if (!userCheckResult.Success) return BadRequest("No user with this id exists.");

        //var invoiceCheckResult = await _invoiceCheck.InvoiceExistanceCheck(updateForm.InvoiceId);
        //if (!invoiceCheckResult.Success) return BadRequest("No invoice with this id exists.");


        var result = await _sender.SendUpdateAsync(updateForm);
        if (!result) { return BadRequest("Could not send the ticketupdate request."); }

        return Ok("Ticketupdate request sent.");
    }

    //DELETE
    [HttpDelete]
    [SwaggerOperation(Summary = "Deletes a specific events schedule if the given data is valid.")]
    [SwaggerResponse(200, "Request was sent to the service bus for deletion.")]
    [SwaggerResponse(400, "The data sent is invalid.")]
    public async Task<IActionResult> DeleteTicket(TicketUserEventSeatKey key)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // External checks:
        var eventCheckResult = await _eventCheck.EventExistanceCheck(key.EventId);
        if (!eventCheckResult.Success) return BadRequest("No event with this id exists.");

        //var userCheckResult = await _userCheck.UserExistanceCheck(key.UserId);
        //if (!userCheckResult.Success) return BadRequest("No user with this id exists.");

        var result = await _sender.SendDeleteAsync(key);
        if (!result) { return BadRequest("Could not send the ticketdeletion request."); }

        return Ok("Ticketdeletion request sent.");
    }





    //GET
    // Getting all the tickets that the user has for all events.
    [HttpGet("user/{userId}")]
    [SwaggerOperation(Summary = "Gets all the tickets at all events for a user.")]
    [SwaggerResponse(200, "Returns a list of tickets with that userId.")]
    [SwaggerResponse(400, "The user does not exist.")]
    [SwaggerResponse(404, "No tickets exists for that user.")]
    public async Task<IActionResult> GetAllUsersTickets(string userId)
    {
        //var userCheckResult = await _userCheck.UserExistanceCheck(userId);
        //if (!userCheckResult.Success) return BadRequest("No user with this id exists.");

        var response = await _httpClient.GetAsync($"{_ticketServiceUrl}/user/{userId}");
        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

        var tickets = await response.Content.ReadFromJsonAsync<List<Ticket>>();
        return Ok(tickets);
    }

    // Getting all the tickets for an event.
    [HttpGet("event/{eventId}")]
    [SwaggerOperation(Summary = "Gets all tickets at a certain event.")]
    [SwaggerResponse(200, "Returns a list of all tickets that exists at that event.")]
    [SwaggerResponse(400, "The eventid does not exist.")]
    public async Task<IActionResult> GetAllEventTickets(string eventId)
    {
        // External checks:
        var eventCheckResult = await _eventCheck.EventExistanceCheck(eventId);
        if (!eventCheckResult.Success) return BadRequest("No event with this id exists.");

        var response = await _httpClient.GetAsync($"{_ticketServiceUrl}/event/{eventId}");
        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

        var tickets = await response.Content.ReadFromJsonAsync<List<Ticket>>();
        return Ok(tickets);
    }

    // Getting all the tickets of the user from that event.
    [HttpGet("user/{userId}/event/{eventId}")]
    [SwaggerOperation(Summary = "Gets all the tickets a specific user has at an event.")]
    [SwaggerResponse(200, "Returns a list of tickets with that user and event id.")]
    [SwaggerResponse(400, "Either the eventid or userid sent does not exist.")]
    [SwaggerResponse(400, "Either the eventid or userid was invalid.")]
    [SwaggerResponse(404, "This user does not have any tickets at this event.")]
    public async Task<IActionResult> GetAllUsersTicketsAtEvent(string userId, string eventId)
    {
        // External checks:
        var eventCheckResult = await _eventCheck.EventExistanceCheck(eventId);
        if (!eventCheckResult.Success) return BadRequest("No event with this id exists.");

        //var userCheckResult = await _userCheck.UserExistanceCheck(userId);
        //if (!userCheckResult.Success) return BadRequest("No user with this id exists.");

        // GET to ticketservice:
        var response = await _httpClient.GetAsync($"{_ticketServiceUrl}/user/{userId}/event/{eventId}");
        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

        var tickets = await response.Content.ReadFromJsonAsync<List<Ticket>>();

        return Ok(tickets);
    }

    // Getting one single ticket for a user at one event.
    [HttpGet("user/{userId}/event/{eventId}/seat/{seatNumber}")]
    [SwaggerOperation(Summary = "Gets a single ticket of a user at a specific event.")]
    [SwaggerResponse(200, "Returns a ticket for that specific userid/eventid and seatnumber.")]
    [SwaggerResponse(400, "Either the eventid or userid sent does not exist.")]
    [SwaggerResponse(400, "Either the eventid or userid was invalid.")]
    [SwaggerResponse(404, "This user does not have any tickets at this event.")]
    public async Task<IActionResult> GetATicket(string userId, string eventId, string seatNumber)
    {
        // External checks:
        var eventCheckResult = await _eventCheck.EventExistanceCheck(eventId);
        if (!eventCheckResult.Success) return BadRequest("No event with this id exists.");

        //var userCheckResult = await _userCheck.UserExistanceCheck(userId);
        //if (!userCheckResult.Success) return BadRequest("No user with this id exists.");

        // GET to ticketservice:
        var response = await _httpClient.GetAsync($"{_ticketServiceUrl}/user/{userId}/event/{eventId}/seat/{seatNumber}");
        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

        var ticket = await response.Content.ReadFromJsonAsync<Ticket>();

        return Ok(ticket);
    }
}
