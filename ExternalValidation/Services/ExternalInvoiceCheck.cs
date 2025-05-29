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


    // Nothing around invoice exists right now. No way to create this for now.
}
