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
        return new ExternalResponse() { Success = true, Statuscode = 200 };
    }
}
