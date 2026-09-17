using System.Collections.Generic;

namespace MetaFramework.Rewards
{
    public sealed class RewardRegistry : IRewardRegistry
    {
        private readonly Dictionary<string, RewardItemDefinition> _items = new();

        public void Register(RewardItemDefinition definition) => _items[definition.ItemId] = definition;
        public RewardItemDefinition? Get(string itemId) => _items.TryGetValue(itemId, out var d) ? d : null;
        public IEnumerable<RewardItemDefinition> GetAll() => _items.Values;
        public bool IsRegistered(string itemId) => _items.ContainsKey(itemId);
    }
}
