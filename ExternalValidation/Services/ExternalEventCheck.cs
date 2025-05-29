using ExternalValidation.Interfaces;
using ExternalValidation.Poco;
using ExternalValidation.ApiSettings;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ExternalValidation.Services;

public class ExternalEventCheck : IExternalEventCheck
{
    private readonly HttpClient _httpClient;
    private readonly string _eventApiUrl;

    // My validation services are made with the help of chatgpt. 

    public ExternalEventCheck(HttpClient httpClient, IOptions<EventApiSettings> options)
    {
        _httpClient = httpClient;
        _eventApiUrl = options.Value.Url;
    }

    public async Task<ExternalResponse> EventExistanceCheck(string eventId)
    {
        var response = await _httpClient.GetAsync($"{_eventApiUrl}/events");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var events = JsonSerializer.Deserialize<List<Event>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (events == null) { return new ExternalResponse() { Success = false, Message = "External eventslist is null.", Statuscode = 400 }; }

        if (!events.Any(e => e.Id.ToString() == eventId)) { return new ExternalResponse() { Success = false, Message = "No event with that id exists.", Statuscode = 404 }; }

        return new ExternalResponse() { Success = true, Statuscode = 200 };
    }
}
