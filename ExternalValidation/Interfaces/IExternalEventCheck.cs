using ExternalValidation.Models;

namespace ExternalValidation.Interfaces
{
    public interface IExternalEventCheck
    {
        Task<ExternalResponse> EventExistanceCheck(string eventId);
    }
}