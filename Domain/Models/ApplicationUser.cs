using Microsoft.AspNetCore.Identity;

namespace InternalManagementSystem.Domain.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string FullName { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
    }
}
