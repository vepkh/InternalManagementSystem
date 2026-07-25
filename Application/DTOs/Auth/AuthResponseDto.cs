namespace InternalManagementSystem.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
