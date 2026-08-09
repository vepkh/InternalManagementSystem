using System.ComponentModel.DataAnnotations;

namespace InternalManagementSystem.Application.DTOs.Projects
{
    public class AddProjectMemberDto
    {
        [Required]
        public string UserId { get; set; } = null!;
    }
}
