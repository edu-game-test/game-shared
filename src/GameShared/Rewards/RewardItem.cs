namespace MetaFramework.Rewards
{
    public sealed class RewardItem
    {
        public RewardItem() { }
        public RewardItem(string itemId, int amount) { ItemId = itemId; Amount = amount; }
        public string ItemId { get; init; } = string.Empty;
        public int Amount { get; init; }
    }
}
