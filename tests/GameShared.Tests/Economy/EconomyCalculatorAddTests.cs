using MetaFramework.Economy;
using Xunit;

namespace GameShared.Tests.Economy
{
    public class EconomyCalculatorAddTests
    {
        private static readonly ResourceDefinition Coins = new() { Id = "coins", Type = ResourceType.SoftCurrency };
        private static readonly ResourceDefinition Lives = new()
        {
            Id = "lives", Type = ResourceType.Energy, Cap = 5, OverflowAllowed = true,
            Regeneration = new RegenerationConfig { IntervalSeconds = 1800 }
        };
        private static readonly ResourceDefinition Hammer = new() { Id = "booster_hammer", Type = ResourceType.Booster, Cap = 10 };
        private static readonly ResourceDefinition Keys = new() { Id = "keys", Type = ResourceType.GatedResource, Cap = 3, CapPolicy = CapExceededPolicy.Reject };

        [Fact]
        public void NoCap_AddsFreely()
        {
            var r = EconomyCalculator.Add(new EconomyEntry { ResourceId = "coins", Amount = 100 }, Coins, 250);
            Assert.Equal(EconomyStatus.Success, r.Status);
            Assert.Equal(350, r.Entry.Amount);
            Assert.Equal(250, r.AmountApplied);
        }

        [Fact]
        public void WithinCap_AddsNormally()
        {
            var r = EconomyCalculator.Add(new EconomyEntry { ResourceId = "lives", Amount = 3 }, Lives, 1);
            Assert.Equal(EconomyStatus.Success, r.Status);
            Assert.Equal(4, r.Entry.Amount);
            Assert.Equal(0, r.Entry.OverflowAmount);
        }

        [Fact]
        public void OverCap_OverflowAllowed_StoresExcessInOverflow()
        {
            var r = EconomyCalculator.Add(new EconomyEntry { ResourceId = "lives", Amount = 3 }, Lives, 5);
            Assert.Equal(EconomyStatus.Success, r.Status);
            Assert.Equal(5, r.Entry.Amount);
            Assert.Equal(3, r.Entry.OverflowAmount);
            Assert.Equal(8, r.Entry.Total);
            Assert.Equal(5, r.AmountApplied);
        }

        [Fact]
        public void OverCap_AlreadyInOverflow_AddsToOverflow()
        {
            var r = EconomyCalculator.Add(new EconomyEntry { ResourceId = "lives", Amount = 5, OverflowAmount = 2 }, Lives, 1);
            Assert.Equal(5, r.Entry.Amount);
            Assert.Equal(3, r.Entry.OverflowAmount);
        }

        [Fact]
        public void OverCap_Clamp_DiscardsRemainder()
        {
            var r = EconomyCalculator.Add(new EconomyEntry { ResourceId = "booster_hammer", Amount = 8 }, Hammer, 5);
            Assert.Equal(EconomyStatus.Capped, r.Status);
            Assert.Equal(10, r.Entry.Amount);
            Assert.Equal(2, r.AmountApplied);
            Assert.Equal(3, r.AmountDiscarded);
        }

        [Fact]
        public void OverCap_Reject_LeavesEntryUnchanged()
        {
            var entry = new EconomyEntry { ResourceId = "keys", Amount = 2 };
            var r = EconomyCalculator.Add(entry, Keys, 2);
            Assert.Equal(EconomyStatus.Rejected, r.Status);
            Assert.Same(entry, r.Entry);
            Assert.Equal(0, r.AmountApplied);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void NonPositiveDelta_IsInvalid(int delta)
        {
            var r = EconomyCalculator.Add(new EconomyEntry { ResourceId = "coins" }, Coins, delta);
            Assert.Equal(EconomyStatus.InvalidDelta, r.Status);
        }

        [Fact]
        public void Add_ToRegeneratingResource_SnapshotsInterval()
        {
            var r = EconomyCalculator.Add(new EconomyEntry { ResourceId = "lives", Amount = 1 }, Lives, 1);
            Assert.Equal(1800, r.Entry.RegenIntervalSeconds);
        }
    }
}
