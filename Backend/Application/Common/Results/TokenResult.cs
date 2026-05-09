namespace DBS_Task.Application.Common.Results
{
        public class TokenResult
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
            public DateTime AccessTokenExpiresAt { get; set; }
            public DateTime RefreshTokenExpiresAt { get; set; }
            public List<string> Roles { get; set; }
            public List<string> Claims { get; set; }
        }
}
