using ExternalValidation.Interfaces;
using ExternalValidation.Poco;
using ExternalValidation.ApiSettings;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ExternalValidation.Services;

public class ExternalUserCheck : IExternalUserCheck
{
    // Major changes maybe needed seeing as this might be a gRPC that I need to contact.
    // Awaiting documentation before making a bunch of changes.


    private readonly HttpClient _httpClient;
    private readonly string _userApiUrl;

    public ExternalUserCheck(HttpClient httpClient, IOptions<UserApiSettings> options)
    {
        _httpClient = httpClient;
        _userApiUrl = options.Value.Url;
    }

    public async Task<ExternalResponse> UserExistanceCheck(string userId)
    {
        var response = await _httpClient.GetAsync($"{_userApiUrl}/");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var users = JsonSerializer.Deserialize<List<User>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (users == null) { return new ExternalResponse() { Success = false, Message = "External userlist is null.", Statuscode = 400 }; }

        if (!users.Any(u => u.Id.ToString() == userId)) { return new ExternalResponse() { Success = false, Message = "No user with that id exists.", Statuscode = 404 }; }

        return new ExternalResponse() { Success = true, Statuscode = 200 };
    }
}
