using InternalManagementSystem.Application.DTOs.Users;

namespace InternalManagementSystem.Application.Interfaces
{
    public interface IUserService
    {
        Task<List<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto> UpdateUserRoleAsync(string userId, string newRole);

        Task<UserResponseDto> DeactivateUserAsync(string userId);
        Task<UserResponseDto> ReactivateUserAsync(string userId);
    }
}
