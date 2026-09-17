using System.Collections.Generic;

namespace MetaFramework.Rewards
{
    public sealed class GrantResult
    {
        public string GrantId { get; init; } = string.Empty;
        public string IdempotencyKey { get; init; } = string.Empty;
        public bool WasDuplicate { get; init; }
        public List<ResolvedRewardItem> ResolvedItems { get; init; } = new();
    }
}
