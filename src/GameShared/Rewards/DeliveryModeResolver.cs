using System.Collections.Generic;
using System.Linq;
using MetaFramework.Common;

namespace MetaFramework.Rewards
{
    /// <summary>Delivery-mode rules and bundle sanitation from reward-system.md.</summary>
    public static class DeliveryModeResolver
    {
        /// <summary>
        /// Priority: (1) any item definition has IsDeliveredViaInbox → Inbox; (2) Source in inboxSources → Inbox; (3) Direct.
        /// </summary>
        public static DeliveryMode Resolve(RewardBundle bundle, IRewardRegistry registry, ISet<string> inboxSources)
        {
            if (bundle.Items.Any(i => registry.Get(i.ItemId)?.IsDeliveredViaInbox == true))
                return DeliveryMode.Inbox;
            if (inboxSources.Contains(bundle.Source))
                return DeliveryMode.Inbox;
            return DeliveryMode.Direct;
        }

        /// <summary>Strips Amount ≤ 0 items silently and unregistered items with an error log. Never throws.</summary>
        public static RewardBundle Sanitize(RewardBundle bundle, IRewardRegistry registry, ILog log)
        {
            var items = new List<RewardItem>();
            foreach (var item in bundle.Items)
            {
                if (item.Amount <= 0) continue;
                if (!registry.IsRegistered(item.ItemId))
                {
                    log.Error($"[Rewards] Bundle '{bundle.BundleId}' references unregistered item '{item.ItemId}'; skipping it.");
                    continue;
                }
                items.Add(item);
            }

            return new RewardBundle
            {
                BundleId = bundle.BundleId,
                IdempotencyKey = bundle.IdempotencyKey,
                Items = items,
                Source = bundle.Source,
                ChestTier = bundle.ChestTier,
                AnimationSpeedMultiplier = bundle.AnimationSpeedMultiplier,
            };
        }
    }
}
