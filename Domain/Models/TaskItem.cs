namespace InternalManagementSystem.Domain.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string? AssignedToUserId { get; set; }
        public string CreatedByUserId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string Status { get; set; } = "Pending";
        public string? Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        public Project Project { get; set; } = null!;
        public ApplicationUser? AssignedToUser { get; set; }
        public ApplicationUser CreatedByUser { get; set; } = null!;
    }
}
