namespace InternalManagementSystem.Application.DTOs.Projects
{
    public class ProjectResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = null!;
        public string ManagerId { get; set; } = null!;
        public string ManagerName { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<string> MemberNames { get; set; } = new();
    }
}
