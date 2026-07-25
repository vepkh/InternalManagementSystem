using InternalManagementSystem.Domain.Models;

namespace InternalManagementSystem.Application.Interfaces
{
    public interface IJwtService
    {
        Task<string> GenerateAccessTokenAsync(ApplicationUser user);
        string GenerateRefreshToken();
        DateTime GetAccessTokenExpiry();

    }
}
