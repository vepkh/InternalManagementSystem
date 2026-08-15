namespace InternalManagementSystem.Application.DTOs.Tasks
{
    public class TaskResponseDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = null!;
        public string? AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }
        public string CreatedByUserId { get; set; } = null!;
        public string CreatedByUserName { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string Status { get; set; } = null!;
        public string? Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
