namespace MetaFramework.FeatureFlags
{
    public sealed class FlagSegmentValue
    {
        public string Segment { get; set; } = string.Empty;
        /// <summary>Higher wins when a player is in several segments with values.</summary>
        public int Priority { get; set; }
        public object? Value { get; set; }
    }
}
