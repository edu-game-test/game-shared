namespace MetaFramework.Economy
{
    public sealed class RegenerationConfig
    {
        public int IntervalSeconds { get; init; }
        public int RegenAmount { get; init; } = 1;
    }
}
