using System;

namespace MetaFramework.Economy
{
    /// <summary>
    /// Pure add / spend / regen rules from economy-system.md. Stateless; callers persist the returned Entry.
    /// Server runs these inside a Firestore transaction; client runs them for optimistic UI.
    /// </summary>
    public static class EconomyCalculator
    {
        public static EconomyOutcome Add(EconomyEntry entry, ResourceDefinition def, int delta)
        {
            if (delta <= 0)
                return Fail(entry, EconomyStatus.InvalidDelta);

            var snapshotInterval = def.Regeneration?.IntervalSeconds ?? entry.RegenIntervalSeconds;

            if (def.Cap == null)
                return Ok(entry.With(amount: entry.Amount + delta, regenInterval: snapshotInterval), delta);

            var cap = def.Cap.Value;
            var newTotal = entry.Total + delta;

            if (newTotal <= cap)
                return Ok(entry.With(amount: entry.Amount + delta, regenInterval: snapshotInterval), delta);

            if (def.OverflowAllowed)
                return Ok(entry.With(amount: cap, overflow: newTotal - cap, regenInterval: snapshotInterval), delta);

            if (def.CapPolicy == CapExceededPolicy.Reject)
                return Fail(entry, EconomyStatus.Rejected);

            var applied = Math.Max(0, cap - entry.Total);
            return new EconomyOutcome
            {
                Status = EconomyStatus.Capped,
                Entry = entry.With(amount: cap, overflow: 0, regenInterval: snapshotInterval),
                AmountApplied = applied,
                AmountDiscarded = delta - applied,
            };
        }

        /// <param name="now">Server time; used to restart the regen timer when leaving FULL / overflow state.</param>
        public static EconomyOutcome Spend(EconomyEntry entry, ResourceDefinition def, int amount, DateTime now)
        {
            if (amount <= 0)
                return Fail(entry, EconomyStatus.InvalidDelta);
            if (entry.Total < amount)
                return Fail(entry, EconomyStatus.InsufficientFunds);

            var fromOverflow = Math.Min(amount, entry.OverflowAmount);
            var fromBase = amount - fromOverflow;
            var wasPaused = entry.IsRegenPaused(def);

            var next = entry.With(amount: entry.Amount - fromBase, overflow: entry.OverflowAmount - fromOverflow);

            // Leaving a paused state (FULL or overflow) restarts the timer from now — the paused time never counts.
            if (def.Regeneration != null && wasPaused && !next.IsRegenPaused(def))
                next = next.With(lastRegen: now, regenInterval: def.Regeneration.IntervalSeconds);

            return Ok(next, amount);
        }

        public static EconomyOutcome ApplyRegen(EconomyEntry entry, ResourceDefinition def, DateTime now)
        {
            if (def.Regeneration == null || def.Cap == null)
                return NoChange(entry);

            if (entry.LastRegenTimestamp == null)
                return NoChange(entry.With(lastRegen: now, regenInterval: def.Regeneration.IntervalSeconds));

            if (entry.IsRegenPaused(def))
                return NoChange(entry);

            var interval = entry.RegenIntervalSeconds ?? def.Regeneration.IntervalSeconds;
            if (interval <= 0)
                return NoChange(entry);

            var elapsedSeconds = (long)(now - entry.LastRegenTimestamp.Value).TotalSeconds;
            var intervals = (int)Math.Floor(elapsedSeconds / (double)interval);
            if (intervals <= 0)
                return NoChange(entry);

            var perInterval = Math.Max(1, def.Regeneration.RegenAmount);
            var headroom = def.Cap.Value - entry.Amount;
            var regen = Math.Min(intervals * perInterval, headroom);
            if (regen <= 0)
                return NoChange(entry);

            var intervalsConsumed = (regen + perInterval - 1) / perInterval;
            var next = entry.With(
                amount: entry.Amount + regen,
                lastRegen: entry.LastRegenTimestamp.Value.AddSeconds((double)intervalsConsumed * interval),
                regenInterval: def.Regeneration.IntervalSeconds);

            return Ok(next, regen);
        }

        private static EconomyOutcome Ok(EconomyEntry entry, int applied) =>
            new() { Status = EconomyStatus.Success, Entry = entry, AmountApplied = applied };

        private static EconomyOutcome NoChange(EconomyEntry entry) =>
            new() { Status = EconomyStatus.NoChange, Entry = entry };

        private static EconomyOutcome Fail(EconomyEntry entry, EconomyStatus status) =>
            new() { Status = status, Entry = entry };
    }
}
