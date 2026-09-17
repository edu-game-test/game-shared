using System.Collections.Generic;

namespace MetaFramework.Rewards
{
    public sealed class RewardBundle
    {
        /// <summary>Bundle definition id, e.g. "level_win_bundle_001".</summary>
        public string BundleId { get; init; } = string.Empty;
        /// <summary>Server-issued UUID. Same key twice → original result, no re-grant.</summary>
        public string IdempotencyKey { get; init; } = string.Empty;
        public List<RewardItem> Items { get; init; } = new();
        /// <summary>Feature tag: "level_win", "daily_bonus_day_3", "iap_purchase", …</summary>
        public string Source { get; init; } = string.Empty;
        public ChestTier ChestTier { get; init; }
        public float AnimationSpeedMultiplier { get; init; } = 1.0f;
    }
}
