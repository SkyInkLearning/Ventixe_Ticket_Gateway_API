using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using System.Text.Json;
using TicketGateway.Models;

namespace TicketGateway.Services;

public class TicketSBSender : IAsyncDisposable
{
    // Service which will send POST/PUT/DELETE to the service bus.
    // Created with alot of help from chatgpt.

    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;

    public TicketSBSender(IOptions<AzureServiceBusSettings> settings)
    {
        var config = settings.Value;
        _client = new ServiceBusClient(config.ConnectionString);
        _sender = _client.CreateSender(config.QueueName);
    }

    public async Task<bool> SendCreateAsync(CreateTicketForm createTicketForm)
    {
        return await SendMessageAsync("CreateTicket", createTicketForm);
    }
    public async Task<bool> SendUpdateAsync(UpdateTicketForm updateTicketForm)
    {
        return await SendMessageAsync("UpdateTicket", updateTicketForm);
    }
    public async Task<bool> SendDeleteAsync(TicketUserEventSeatKey deleteKey)
    {
        return await SendMessageAsync("DeleteTicket", deleteKey);
    }

    private async Task<bool> SendMessageAsync(string type, object payload)
    {
        try
        {
            var wrapper = new
            {
                Type = type,
                Payload = payload
            };

            var json = JsonSerializer.Serialize(wrapper);
            var message = new ServiceBusMessage(json);
            await _sender.SendMessageAsync(message);
            return true;
        }
        catch (Exception ex) { return false; }
    }

    public async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _client.DisposeAsync();
    }
}
