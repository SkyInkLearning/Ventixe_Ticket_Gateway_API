using ExternalValidation.Poco;

namespace ExternalValidation.Interfaces
{
    public interface IExternalEventCheck
    {
        Task<ExternalResponse> EventExistanceCheck(string eventId);
    }
}