namespace MetaFramework.Segmentation
{
    public sealed class SplitTestVariant
    {
        public string Name { get; set; } = string.Empty;
        /// <summary>[inclusiveLow, inclusiveHigh] within 0–99.</summary>
        public int[] BucketRange { get; set; } = { 0, 99 };
    }
}
