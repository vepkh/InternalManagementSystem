using System.ComponentModel.DataAnnotations;

namespace InternalManagementSystem.Application.DTOs.Tasks
{
    public class ChangeTaskStatusDto
    {
        [Required]
        public string Status { get; set; } = null!;
    }
}
