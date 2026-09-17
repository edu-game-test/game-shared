using System;
using MetaFramework.Economy;
using Xunit;

namespace GameShared.Tests.Economy
{
    public class EconomyCalculatorRegenTests
    {
        private static readonly DateTime T0 = new(2026, 9, 10, 12, 0, 0, DateTimeKind.Utc);
        private static readonly ResourceDefinition Lives = new()
        {
            Id = "lives", Type = ResourceType.Energy, Cap = 5, OverflowAllowed = true,
            Regeneration = new RegenerationConfig { IntervalSeconds = 1800 }
        };
        private static readonly ResourceDefinition Coins = new() { Id = "coins", Type = ResourceType.SoftCurrency };

        [Fact]
        public void TwoPointSevenIntervals_RegensTwo_KeepsFractionalCarry()
        {
            var entry = new EconomyEntry { ResourceId = "lives", Amount = 1, LastRegenTimestamp = T0, RegenIntervalSeconds = 1800 };
            var now = T0.AddSeconds(2.7 * 1800);

            var r = EconomyCalculator.ApplyRegen(entry, Lives, now);

            Assert.Equal(EconomyStatus.Success, r.Status);
            Assert.Equal(3, r.Entry.Amount);
            Assert.Equal(2, r.AmountApplied);
            Assert.Equal(T0.AddSeconds(2 * 1800), r.Entry.LastRegenTimestamp);
        }

        [Fact]
        public void NeverRegensPastCap()
        {
            var entry = new EconomyEntry { ResourceId = "lives", Amount = 4, LastRegenTimestamp = T0, RegenIntervalSeconds = 1800 };
            var r = EconomyCalculator.ApplyRegen(entry, Lives, T0.AddHours(10));
            Assert.Equal(5, r.Entry.Amount);
            Assert.Equal(1, r.AmountApplied);
        }

        [Fact]
        public void InOverflow_IsPaused()
        {
            var entry = new EconomyEntry { ResourceId = "lives", Amount = 5, OverflowAmount = 2, LastRegenTimestamp = T0, RegenIntervalSeconds = 1800 };
            var r = EconomyCalculator.ApplyRegen(entry, Lives, T0.AddHours(10));
            Assert.Equal(EconomyStatus.NoChange, r.Status);
            Assert.Equal(0, r.AmountApplied);
            Assert.Same(entry, r.Entry);
        }

        [Fact]
        public void LessThanOneInterval_NoChange()
        {
            var entry = new EconomyEntry { ResourceId = "lives", Amount = 2, LastRegenTimestamp = T0, RegenIntervalSeconds = 1800 };
            var r = EconomyCalculator.ApplyRegen(entry, Lives, T0.AddMinutes(29));
            Assert.Equal(EconomyStatus.NoChange, r.Status);
            Assert.Equal(T0, r.Entry.LastRegenTimestamp);
        }

        [Fact]
        public void NonRegenerating_NoChange()
        {
            var entry = new EconomyEntry { ResourceId = "coins", Amount = 2 };
            var r = EconomyCalculator.ApplyRegen(entry, Coins, T0.AddDays(1));
            Assert.Equal(EconomyStatus.NoChange, r.Status);
        }

        [Fact]
        public void MissingTimestamp_AnchorsToNow()
        {
            var entry = new EconomyEntry { ResourceId = "lives", Amount = 2 };
            var r = EconomyCalculator.ApplyRegen(entry, Lives, T0);
            Assert.Equal(EconomyStatus.NoChange, r.Status);
            Assert.Equal(T0, r.Entry.LastRegenTimestamp);
        }

        [Fact]
        public void UsesSnapshotInterval_NotDefinition()
        {
            var entry = new EconomyEntry { ResourceId = "lives", Amount = 0, LastRegenTimestamp = T0, RegenIntervalSeconds = 600 };
            var r = EconomyCalculator.ApplyRegen(entry, Lives, T0.AddMinutes(30));
            Assert.Equal(3, r.Entry.Amount);
            Assert.Equal(1800, r.Entry.RegenIntervalSeconds);
        }
    }
}
