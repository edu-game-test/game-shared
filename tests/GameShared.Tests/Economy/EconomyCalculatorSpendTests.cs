using System;
using MetaFramework.Economy;
using Xunit;

namespace GameShared.Tests.Economy
{
    public class EconomyCalculatorSpendTests
    {
        private static readonly DateTime Now = new(2026, 9, 10, 12, 0, 0, DateTimeKind.Utc);
        private static readonly ResourceDefinition Lives = new()
        {
            Id = "lives", Type = ResourceType.Energy, Cap = 5, OverflowAllowed = true,
            Regeneration = new RegenerationConfig { IntervalSeconds = 1800 }
        };
        private static readonly ResourceDefinition Coins = new() { Id = "coins", Type = ResourceType.SoftCurrency };

        [Fact]
        public void Spend_ConsumesOverflowFirst()
        {
            var r = EconomyCalculator.Spend(new EconomyEntry { ResourceId = "lives", Amount = 5, OverflowAmount = 3 }, Lives, 1, Now);
            Assert.Equal(EconomyStatus.Success, r.Status);
            Assert.Equal(5, r.Entry.Amount);
            Assert.Equal(2, r.Entry.OverflowAmount);
        }

        [Fact]
        public void Spend_MoreThanOverflow_DipsIntoBase()
        {
            var r = EconomyCalculator.Spend(new EconomyEntry { ResourceId = "lives", Amount = 5, OverflowAmount = 3 }, Lives, 4, Now);
            Assert.Equal(4, r.Entry.Amount);
            Assert.Equal(0, r.Entry.OverflowAmount);
            Assert.Equal(4, r.AmountApplied);
        }

        [Fact]
        public void Spend_InsufficientFunds_LeavesEntryUnchanged()
        {
            var entry = new EconomyEntry { ResourceId = "coins", Amount = 10 };
            var r = EconomyCalculator.Spend(entry, Coins, 11, Now);
            Assert.Equal(EconomyStatus.InsufficientFunds, r.Status);
            Assert.Same(entry, r.Entry);
        }

        [Fact]
        public void Spend_ExactBalance_GoesToZero()
        {
            var r = EconomyCalculator.Spend(new EconomyEntry { ResourceId = "coins", Amount = 10 }, Coins, 10, Now);
            Assert.Equal(0, r.Entry.Amount);
        }

        [Fact]
        public void Spend_FromFull_RestartsRegenTimerAtNow()
        {
            var full = new EconomyEntry { ResourceId = "lives", Amount = 5, LastRegenTimestamp = Now.AddHours(-3) };
            var r = EconomyCalculator.Spend(full, Lives, 1, Now);
            Assert.Equal(Now, r.Entry.LastRegenTimestamp);
        }

        [Fact]
        public void Spend_WhileTimerRunning_KeepsTimestamp()
        {
            var anchor = Now.AddMinutes(-10);
            var running = new EconomyEntry { ResourceId = "lives", Amount = 3, LastRegenTimestamp = anchor };
            var r = EconomyCalculator.Spend(running, Lives, 1, Now);
            Assert.Equal(anchor, r.Entry.LastRegenTimestamp);
        }

        [Fact]
        public void Spend_DepletingOverflow_ResumesTimerAtNow()
        {
            var overflowed = new EconomyEntry { ResourceId = "lives", Amount = 4, OverflowAmount = 1, LastRegenTimestamp = Now.AddHours(-3) };
            var r = EconomyCalculator.Spend(overflowed, Lives, 1, Now);
            Assert.Equal(0, r.Entry.OverflowAmount);
            Assert.Equal(Now, r.Entry.LastRegenTimestamp);
        }

        [Fact]
        public void Spend_NonPositive_IsInvalid()
        {
            var r = EconomyCalculator.Spend(new EconomyEntry { ResourceId = "coins", Amount = 10 }, Coins, 0, Now);
            Assert.Equal(EconomyStatus.InvalidDelta, r.Status);
        }
    }
}
