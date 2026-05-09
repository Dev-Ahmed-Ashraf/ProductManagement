using Microsoft.AspNetCore.Identity;

namespace DBS_Task.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName {  get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public ICollection<RefreshToken> RefreshTokens
        { get; set; } = new List<RefreshToken>();

    }
}
