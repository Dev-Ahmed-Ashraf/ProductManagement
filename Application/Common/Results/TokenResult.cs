namespace DBS_Task.Application.Common.Results
{
        public class TokenResult
        {
            public string Token { get; set; }

            public DateTime ExpiresAt { get; set; }

            public List<string> Roles { get; set; }

            public List<string> Claims { get; set; }
        }
}
