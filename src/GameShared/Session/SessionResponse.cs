using System;
using System.Collections.Generic;

namespace MetaFramework.Session
{
    public sealed class SessionResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresInSeconds { get; set; }
        public string PlayerId { get; set; } = string.Empty;
        public bool IsNewPlayer { get; set; }
        public bool IsGuest { get; set; }
        public List<string> Segments { get; set; } = new();
        public Dictionary<string, string> AbAssignments { get; set; } = new();
        public int SegmentSetVersion { get; set; }
        public DateTime ServerTimeUtc { get; set; }
        /// <summary>Reserved for the Skinning System (Phase 2). Null on Day 1.</summary>
        public string? ActiveSkinId { get; set; }
    }
}
