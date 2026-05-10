using Microsoft.AspNetCore.Identity;

namespace DBS_Task.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName {  get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<RefreshToken> RefreshTokens
        { get; set; } = new List<RefreshToken>();

    }
}
