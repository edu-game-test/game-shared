namespace MetaFramework.FeatureFlags
{
    public sealed class FlagRollout
    {
        /// <summary>0–100. bucket &lt; Percentage → EnabledValue.</summary>
        public int Percentage { get; set; }
        public object? EnabledValue { get; set; }
        public object? DisabledValue { get; set; }
    }
}
