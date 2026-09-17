using System;

namespace MetaFramework.Economy
{
    public sealed class TransactionResult
    {
        public bool Success { get; init; }
        public EconomyError? Error { get; init; }
        public int NewAmount { get; init; }
        public int NewOverflow { get; init; }
        public Guid TransactionId { get; init; }
    }
}
