namespace MetaFramework.Rewards
{
    public sealed class RewardItemDefinition
    {
        public string ItemId { get; init; } = string.Empty;
        /// <summary>Localization key.</summary>
        public string DisplayName { get; init; } = string.Empty;
        public string IconAssetKey { get; init; } = string.Empty;
        public string AnimationKey { get; init; } = string.Empty;
        public RewardItemType Type { get; init; }
        /// <summary>When true the whole bundle routes through the Inbox.</summary>
        public bool IsDeliveredViaInbox { get; init; }
        public OverflowRule OverflowRule { get; init; }
    }
}
