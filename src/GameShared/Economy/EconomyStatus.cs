namespace MetaFramework.Economy
{
    public enum EconomyStatus
    {
        Success,
        /// <summary>Add was clamped to the cap; AmountDiscarded holds the remainder.</summary>
        Capped,
        /// <summary>Add rejected because cap exceeded and CapPolicy is Reject. Entry unchanged.</summary>
        Rejected,
        InsufficientFunds,
        InvalidDelta,
        NoChange,
    }
}
