using Microsoft.AspNetCore.Identity;

namespace DBS_Task.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName {  get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public bool IsActive { get; private set; } = true;
    }
}
