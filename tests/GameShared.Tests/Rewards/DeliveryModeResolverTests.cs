using System.Collections.Generic;
using GameShared.Tests.Fakes;
using MetaFramework.Rewards;
using Xunit;

namespace GameShared.Tests.Rewards
{
    public class DeliveryModeResolverTests
    {
        private static RewardRegistry Registry()
        {
            var reg = new RewardRegistry();
            reg.Register(new RewardItemDefinition { ItemId = "coins", Type = RewardItemType.Currency });
            reg.Register(new RewardItemDefinition { ItemId = "lives", Type = RewardItemType.Energy });
            reg.Register(new RewardItemDefinition { ItemId = "legendary_frame", Type = RewardItemType.Cosmetic, IsDeliveredViaInbox = true });
            return reg;
        }

        private static readonly HashSet<string> InboxSources = new() { "event_leaderboard", "inbox_admin", "inbox_system" };

        [Fact]
        public void ItemFlaggedForInbox_RoutesWholeBundleToInbox()
        {
            var bundle = new RewardBundle { Source = "level_win", Items = { new("coins", 100), new("legendary_frame", 1) } };
            Assert.Equal(DeliveryMode.Inbox, DeliveryModeResolver.Resolve(bundle, Registry(), InboxSources));
        }

        [Fact]
        public void SourceInInboxList_RoutesToInbox()
        {
            var bundle = new RewardBundle { Source = "event_leaderboard", Items = { new("coins", 5000) } };
            Assert.Equal(DeliveryMode.Inbox, DeliveryModeResolver.Resolve(bundle, Registry(), InboxSources));
        }

        [Fact]
        public void Default_IsDirect()
        {
            var bundle = new RewardBundle { Source = "level_win", Items = { new("coins", 100), new("lives", 1) } };
            Assert.Equal(DeliveryMode.Direct, DeliveryModeResolver.Resolve(bundle, Registry(), InboxSources));
        }

        [Fact]
        public void Sanitize_StripsZeroAmounts_AndUnregistered_LogsUnregistered()
        {
            var log = new ListLog();
            var bundle = new RewardBundle
            {
                BundleId = "b1", Source = "level_win",
                Items = { new("coins", 100), new("lives", 0), new("unicorn", 3) }
            };

            var clean = DeliveryModeResolver.Sanitize(bundle, Registry(), log);

            Assert.Single(clean.Items);
            Assert.Equal("coins", clean.Items[0].ItemId);
            Assert.Equal("b1", clean.BundleId);
            Assert.Single(log.Errors);
            Assert.Contains("unicorn", log.Errors[0].Message);
        }
    }
}
