using System.Collections.Generic;

namespace MetaFramework.Rewards
{
    public interface IRewardRegistry
    {
        void Register(RewardItemDefinition definition);
        RewardItemDefinition? Get(string itemId);
        IEnumerable<RewardItemDefinition> GetAll();
        bool IsRegistered(string itemId);
    }
}
