using ExternalValidation.Models;

namespace ExternalValidation.Interfaces
{
    public interface IExternalInvoiceCheck
    {
        Task<ExternalResponse> InvoiceExistanceCheck(string invoiceId);
    }
}