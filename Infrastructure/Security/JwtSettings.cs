using System.ComponentModel.DataAnnotations;

namespace DBS_Task.Infrastructure.Security
{
    public class JwtSettings
    {
        [Required, MinLength(32)]
        public string SecretKey { get; set; } = default!;

        [Required]
        public string Issuer { get; set; } = default!;

        [Required]
        public string Audience { get; set; } = default!;

        [Range(1, 1440)]
        public int ExpirationMinutes { get; set; }
    }
}
