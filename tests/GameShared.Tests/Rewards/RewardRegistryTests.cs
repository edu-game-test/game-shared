using MetaFramework.Rewards;
using Xunit;

namespace GameShared.Tests.Rewards
{
    public class RewardRegistryTests
    {
        [Fact]
        public void Register_ThenGet_ReturnsDefinition()
        {
            var reg = new RewardRegistry();
            reg.Register(new RewardItemDefinition { ItemId = "coins", Type = RewardItemType.Currency });
            Assert.True(reg.IsRegistered("coins"));
            Assert.Equal(RewardItemType.Currency, reg.Get("coins")!.Type);
        }

        [Fact]
        public void Get_Unknown_ReturnsNull()
        {
            Assert.Null(new RewardRegistry().Get("nope"));
        }

        [Fact]
        public void Register_Twice_LastWins()
        {
            var reg = new RewardRegistry();
            reg.Register(new RewardItemDefinition { ItemId = "coins", OverflowRule = OverflowRule.Drop });
            reg.Register(new RewardItemDefinition { ItemId = "coins", OverflowRule = OverflowRule.Clamp });
            Assert.Equal(OverflowRule.Clamp, reg.Get("coins")!.OverflowRule);
            Assert.Single(reg.GetAll());
        }
    }
}
