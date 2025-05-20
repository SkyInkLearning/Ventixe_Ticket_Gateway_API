using ExternalValidation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketGateway.Models;
using TicketGateway.Services;

namespace TicketGateway.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TicketGatewayController(TicketSBSender sender, ExternalEventCheck eventCheck, ExternalUserCheck userCheck, ExternalInvoiceCheck invoiceCheck) : ControllerBase
{
    private readonly TicketSBSender _sender = sender;

    private readonly ExternalEventCheck _eventCheck = eventCheck;
    private readonly ExternalUserCheck _userCheck = userCheck;
    private readonly ExternalInvoiceCheck _invoiceCheck = invoiceCheck;


    //POST
    [HttpPost]
    public async Task<IActionResult> CreateTicket(CreateTicketForm createForm)
    {
        // Takes in a form checks the data against external services and then sends it to the SB sender.
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // External checks:
        var eventCheckResult = await _eventCheck.EventExistanceCheck(createForm.EventId);
        if (!eventCheckResult.Success) return BadRequest("No event with this id exists.");

        var userCheckResult = await _userCheck.UserExistanceCheck(createForm.UserId);
        if (!userCheckResult.Success) return BadRequest("No user with this id exists.");

        var invoiceCheckResult = await _invoiceCheck.InvoiceExistanceCheck(createForm.InvoiceId);
        if (!invoiceCheckResult.Success) return BadRequest("No invoice with this id exists.");

        var resultOfSender = await _sender.SendCreateAsync(createForm);
        if (!resultOfSender) { return BadRequest("Could not send the ticketcreation request."); }

        return Ok("Ticketcreation request sent.");
    }

    //PUT
    [HttpPut]
    public async Task<IActionResult> UpdateTicket(UpdateTicketForm updateForm)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // External checks:
        var eventCheckResult = await _eventCheck.EventExistanceCheck(updateForm.EventId);
        if (!eventCheckResult.Success) return BadRequest("No event with this id exists.");

        var userCheckResult = await _userCheck.UserExistanceCheck(updateForm.UserId);
        if (!userCheckResult.Success) return BadRequest("No user with this id exists.");

        var invoiceCheckResult = await _invoiceCheck.InvoiceExistanceCheck(updateForm.InvoiceId);
        if (!invoiceCheckResult.Success) return BadRequest("No invoice with this id exists.");


        var result = await _sender.SendUpdateAsync(updateForm);
        if (!result) { return BadRequest("Could not send the ticketupdate request."); }

        return Ok("Ticketupdate request sent.");
    }

    //DELETE
    [HttpDelete]
    public async Task<IActionResult> DeleteTicket(TicketUserEventSeatKey key)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // External checks:
        var eventCheckResult = await _eventCheck.EventExistanceCheck(key.EventId);
        if (!eventCheckResult.Success) return BadRequest("No event with this id exists.");

        var userCheckResult = await _userCheck.UserExistanceCheck(key.UserId);
        if (!userCheckResult.Success) return BadRequest("No user with this id exists.");

        var result = await _sender.SendDeleteAsync(key);
        if (!result) { return BadRequest("Could not send the ticketdeletion request."); }

        return Ok("Ticketdeletion request sent.");
    }


    //GET
    // Getting all the tickets that the user has for all events.
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetAllUsersTickets(string userId)
    {
        // Need to create a HTTP request thingy to the other api.

        
        return Ok(tickets);
    }

    // Getting all the tickets of the user from that event.
    [HttpGet("user/{userId}/event/{eventId}")]
    public async Task<IActionResult> GetAllUsersTicketsAtEvent(string userId, string eventId)
    {


        return Ok(tickets);
    }

    // Getting one single ticket for a user at one event.
    [HttpGet("user/{userId}/event/{eventId}/seat/{seatNumber}")]
    public async Task<IActionResult> GetATicket(string userId, string eventId, string seatNumber)
    {


        return Ok(ticket);
    }
}
