namespace inetz.authserver.models
{
    public partial class RefreshToken
    {
        public Guid Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
        public string? TokenHash { get; set; }
        public DateTime ExpiresUtc { get; set; }
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow; // 👈 default
        public DateTime? RevokedUtc { get; set; }
        public string ReplacedByTokenHash { get; set; } = string.Empty;
    }
}
