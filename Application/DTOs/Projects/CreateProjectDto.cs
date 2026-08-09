using System.ComponentModel.DataAnnotations;

namespace InternalManagementSystem.Application.DTOs.Projects
{
 

        public class CreateProjectDto
        {
            [Required]
            [MaxLength(200)]
            public string Name { get; set; } = null!;

            public string? Description { get; set; }

            public DateTime? StartDate { get; set; }

            public DateTime? EndDate { get; set; }

            [Required]
            public int ProjectStatusId { get; set; }

            // Only required/used when an Administrator creates the project.
            // Ignored (overridden) when a Manager creates it — they become the manager automatically.
            public string? ManagerId { get; set; }
        }
    
}
