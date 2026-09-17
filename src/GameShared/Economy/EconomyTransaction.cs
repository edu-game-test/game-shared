using System;

namespace MetaFramework.Economy
{
    /// <summary>Immutable ledger record. Corrections are compensating transactions (economy-system.md "Transaction Rules").</summary>
    public sealed class EconomyTransaction
    {
        public Guid TransactionId { get; init; } = Guid.NewGuid();
        public string ResourceId { get; init; } = string.Empty;
        /// <summary>Positive = add, negative = spend.</summary>
        public int Delta { get; init; }
        /// <summary>Allow-listed reason, e.g. "level_complete", "daily_bonus", "iap_purchase", "booster_used", "energy_regen".</summary>
        public string Reason { get; init; } = string.Empty;
        /// <summary>Server UTC when committed. Client-suggested values are overwritten.</summary>
        public DateTime Timestamp { get; init; }
        public int BalanceAfter { get; init; }
        /// <summary>Linked IAP order id or reward bundle id.</summary>
        public string? SourceId { get; init; }
    }
}
