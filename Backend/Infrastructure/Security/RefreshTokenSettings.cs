namespace DBS_Task.Infrastructure.Security
{
    public class RefreshTokenSettings
    {
        public int ExpirationDays { get; set; }

        public int TokenSizeInBytes { get; set; }

        public bool RotateOnUse { get; set; }

        public bool RevokeOnLogout { get; set; }
    }
}
