using System.ComponentModel.DataAnnotations;

namespace InternalManagementSystem.Application.DTOs.Tasks
{
    public class CreateTaskDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [MaxLength(20)]
        public string? Priority { get; set; }

        public DateTime? DueDate { get; set; }

        // Optional at creation — a manager might create a task before deciding who does it
        public string? AssignedToUserId { get; set; }
    }
}
