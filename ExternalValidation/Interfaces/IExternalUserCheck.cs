using ExternalValidation.Models;

namespace ExternalValidation.Interfaces
{
    public interface IExternalUserCheck
    {
        Task<ExternalResponse> UserExistanceCheck(string userId);
    }
}