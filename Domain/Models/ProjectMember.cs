namespace InternalManagementSystem.Domain.Models
{
    public class ProjectMember
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string UserId { get; set; } = null!;
        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;

        public Project Project { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}
