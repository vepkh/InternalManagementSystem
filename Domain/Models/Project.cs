namespace InternalManagementSystem.Domain.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string ManagerId { get; set; } = null!;
        public int ProjectStatusId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        public ApplicationUser Manager { get; set; } = null!;
        public Projectstatus ProjectStatus { get; set; } = null!;
        public ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
