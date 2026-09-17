using System;

namespace MetaFramework.Session
{
    /// <summary>GET /api/v1/player/me</summary>
    public sealed class PlayerProfile
    {
        public string PlayerId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string AvatarId { get; set; } = "default";
        public bool IsGuest { get; set; }
        public string Platform { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string Locale { get; set; } = "en";
        public DateTime InstalledAt { get; set; }
        public DateTime LastLoginAt { get; set; }
    }
}
