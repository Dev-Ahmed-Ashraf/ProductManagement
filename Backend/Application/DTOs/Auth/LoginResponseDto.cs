using DBS_Task.Application.Common.Constants;

namespace DBS_Task.Application.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string userId { get; set; }
        public string fullName { get; set; }
        public string email { get; set; }
        public List<string> roles { get; set; }
        public List<string> claims { get; set; }
        public string accessToken { get; set; }
        public DateTime accessTokenExpiresAt { get; set; }
        public string refreshToken { get; set; }
        public DateTime refreshTokenExpiresAt { get; set; }
    }
}
