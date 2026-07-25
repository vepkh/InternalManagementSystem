namespace InternalManagementSystem.Domain.Models
{
    public class RefreshToken
    {

        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime ExpiresDate { get; set; }
        public bool IsRevoked { get; set; } = false;

        public ApplicationUser User { get; set; } = null!;
    }
}
