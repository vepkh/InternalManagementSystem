using System.ComponentModel.DataAnnotations;

namespace InternalManagementSystem.Application.DTOs.Users
{
    public class UpdateUserRoleDto
    {
        [Required]
        public string NewRole { get; set; } = null!;
    }
}
