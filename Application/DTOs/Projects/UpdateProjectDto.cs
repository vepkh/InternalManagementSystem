using System.ComponentModel.DataAnnotations;

namespace InternalManagementSystem.Application.DTOs.Projects
{
    public class UpdateProjectDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public int ProjectStatusId { get; set; }
    }
}
