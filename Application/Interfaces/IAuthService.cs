using InternalManagementSystem.Application.DTOs.Auth;

namespace InternalManagementSystem.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequestDto);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequestDto);

        Task<AuthResponseDto> RefreshTokenAsync();
        Task LogoutAsync();
    }
}
