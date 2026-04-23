using DBS_Task.Application.Common.Constants;

namespace DBS_Task.Application.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Claims { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
