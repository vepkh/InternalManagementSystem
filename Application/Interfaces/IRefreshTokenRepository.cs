using InternalManagementSystem.Domain.Models;

namespace InternalManagementSystem.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task SaveChangesAsync();
}