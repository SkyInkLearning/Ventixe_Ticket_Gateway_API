using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketGateway.Models;
using TicketGateway.Services;

namespace TicketGateway.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TicketGatewayController : ControllerBase
{
    private readonly TicketSBSender _sender;

    public TicketGatewayController()
    {
        _sender = new TicketSBSender();
    }

    //POST
    [HttpPost]
    public async Task<IActionResult> CreateTicket(CreateTicketForm createForm)
    {
        // Takes in a form, sends checks the data against external services and then sends it to the SB sender.
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // External checks here:






        var result = await _sender.SendCreateAsync(createForm);
        if (!result) { return BadRequest("Could not send the ticketcreation request."); }

        return Ok("Ticketcreation request sent.");
    }

    //PUT
    [HttpPut]
    public async Task<IActionResult> UpdateTicket(UpdateTicketForm updateForm)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _sender.SendUpdateAsync(updateForm);
        if (!result) { return BadRequest("Could not send the ticketupdate request."); }

        return Ok("Ticketupdate request sent.");
    }

    //DELETE
    [HttpDelete]
    public async Task<IActionResult> DeleteTicket(TicketUserEventSeatKey key)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _sender.SendDeleteAsync(key);
        if (!result) { return BadRequest("Could not send the ticketdeletion request."); }

        return Ok("Ticketdeletion request sent.");
    }


    //GET
    // Getting all the tickets that the user has for all events.
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetAllUsersTickets(string userId)
    {
        // Need to validate the userID through external check.
        // Need to create a HTTP request thingy to the other api.

        
        return Ok(tickets);
    }

    // Getting all the tickets of the user from that event.
    [HttpGet("user/{userId}/event/{eventId}")]
    public async Task<IActionResult> GetAllUsersTicketsAtEvent(string userId, string eventId)
    {
        //Need to validate the data through external checks.


        return Ok(tickets);
    }

    // Getting one single ticket for a user at one event.
    [HttpGet("user/{userId}/event/{eventId}/seat/{seatNumber}")]
    public async Task<IActionResult> GetATicket(string userId, string eventId, string seatNumber)
    {
        //Need ot validate data through external checl.


        return Ok(ticket);
    }
}
