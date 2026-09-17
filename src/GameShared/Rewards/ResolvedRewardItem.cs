namespace MetaFramework.Rewards
{
    /// <summary>What the server actually granted for one item after cap handling.</summary>
    public sealed class ResolvedRewardItem
    {
        public string ItemId { get; init; } = string.Empty;
        public int AmountRequested { get; init; }
        public int AmountGranted { get; init; }
        public int AmountDiscarded { get; init; }
        public OverflowRule OverflowRuleApplied { get; init; }
    }
}
