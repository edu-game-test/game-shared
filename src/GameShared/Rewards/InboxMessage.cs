using System;

namespace MetaFramework.Rewards
{
    public sealed class InboxMessage
    {
        public string MessageId { get; init; } = string.Empty;
        public string PlayerId { get; init; } = string.Empty;
        /// <summary>Localization key.</summary>
        public string SubjectKey { get; init; } = string.Empty;
        /// <summary>Localization key.</summary>
        public string BodyKey { get; init; } = string.Empty;
        /// <summary>"system" | "admin" | "event:summer_splash".</summary>
        public string SenderTag { get; init; } = string.Empty;
        /// <summary>Null if the message carries no reward.</summary>
        public RewardBundle? Reward { get; init; }
        public DateTime SentAt { get; init; }
        /// <summary>Null = never expires.</summary>
        public DateTime? ExpiresAt { get; init; }
        public bool IsClaimed { get; init; }
        public bool IsRead { get; init; }
    }
}
