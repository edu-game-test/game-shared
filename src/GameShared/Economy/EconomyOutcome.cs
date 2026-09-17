namespace MetaFramework.Economy
{
    public sealed class EconomyOutcome
    {
        public EconomyStatus Status { get; init; }
        public EconomyEntry Entry { get; init; } = new();
        /// <summary>Units actually added / spent / regenerated.</summary>
        public int AmountApplied { get; init; }
        /// <summary>Units dropped by clamping (Capped) — surfaced to the player by the Reward System.</summary>
        public int AmountDiscarded { get; init; }

        public bool IsSuccess => Status == EconomyStatus.Success || Status == EconomyStatus.Capped || Status == EconomyStatus.NoChange;
    }
}
