using ExternalValidation.Interfaces;
using ExternalValidation.Poco;
using ExternalValidation.ApiSettings;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ExternalValidation.Services;

public class ExternalInvoiceCheck : IExternalInvoiceCheck
{
    private readonly HttpClient _httpClient;
    private readonly string _invoiceApiUrl;

    public ExternalInvoiceCheck(HttpClient httpClient, IOptions<InvoiceApiSettings> options)
    {
        _httpClient = httpClient;
        _invoiceApiUrl = options.Value.Url;
    }

    public async Task<ExternalResponse> InvoiceExistanceCheck(string invoiceId)
    {
        // Need to check the future invoice microservice to figure out how to structure this more.
        // If there is a get one invoice controller then use that.

        var response = await _httpClient.GetAsync($"{_invoiceApiUrl}/");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var invoices = JsonSerializer.Deserialize<List<Invoice>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (invoices == null) { return new ExternalResponse() { Success = false, Message = "External invoice is null.", Statuscode = 400 }; }

        if (!invoices.Any(i => i.Id.ToString() == invoiceId)) { return new ExternalResponse() { Success = false, Message = "No invoice with that id exists.", Statuscode = 404 }; }

        return new ExternalResponse() { Success = true, Statuscode = 200 };
    }
}
