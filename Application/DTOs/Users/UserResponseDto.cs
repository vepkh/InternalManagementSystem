namespace InternalManagementSystem.Application.DTOs.Users
{
   

        public class UserResponseDto
        {
            public string Id { get; set; } = null!;
            public string FullName { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string Role { get; set; } = null!;
            public bool IsActive { get; set; }
        }
    
}
