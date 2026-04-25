using DBS_Task.Domain.Entities.Common;

namespace DBS_Task.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public bool IsRevoked =>
            RevokedAt != null;
        public bool IsExpired =>
            DateTime.UtcNow >= ExpiresAt;
    }
}
