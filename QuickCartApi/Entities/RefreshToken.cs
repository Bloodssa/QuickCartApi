namespace QuickCartApi.Entities
{
    public class RefreshToken : BaseEntity
    {
        public int UserId { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? RevokedAtUtc { get; set; }
        public User? User { get; set; } = null;

        // check if the token is not expired and not revoked or logout
        public bool IsActive 
            => RevokedAtUtc == null && ExpiresAtUtc > DateTime.UtcNow;
    }
}
