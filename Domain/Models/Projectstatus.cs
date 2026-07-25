namespace InternalManagementSystem.Domain.Models
{
    public class Projectstatus
    {
        public int ProjectStatusId { get; set; }
        public string Description { get; set; } = null!;

        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
