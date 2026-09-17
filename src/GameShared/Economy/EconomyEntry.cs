using System;

namespace MetaFramework.Economy
{
    /// <summary>Per-player runtime state for one resource. Immutable; EconomyCalculator returns new instances.</summary>
    public sealed class EconomyEntry
    {
        public string ResourceId { get; init; } = string.Empty;
        /// <summary>Base amount (never above cap).</summary>
        public int Amount { get; init; }
        /// <summary>Amount held above cap (gifts/purchases). Consumed before Amount.</summary>
        public int OverflowAmount { get; init; }
        /// <summary>For regenerating resources: anchor for the regen timer. Null otherwise.</summary>
        public DateTime? LastRegenTimestamp { get; init; }
        /// <summary>Snapshot of the interval at last write so in-flight timers survive config changes.</summary>
        public int? RegenIntervalSeconds { get; init; }

        public int Total => Amount + OverflowAmount;

        /// <summary>Timer is paused while in overflow or at cap (economy-system.md "Regen State Machine").</summary>
        public bool IsRegenPaused(ResourceDefinition def) =>
            def.Regeneration == null || def.Cap == null || OverflowAmount > 0 || Amount >= def.Cap.Value;

        public EconomyEntry With(int? amount = null, int? overflow = null, DateTime? lastRegen = null,
                                 bool clearLastRegen = false, int? regenInterval = null) => new()
        {
            ResourceId = ResourceId,
            Amount = amount ?? Amount,
            OverflowAmount = overflow ?? OverflowAmount,
            LastRegenTimestamp = clearLastRegen ? null : (lastRegen ?? LastRegenTimestamp),
            RegenIntervalSeconds = regenInterval ?? RegenIntervalSeconds,
        };
    }
}
