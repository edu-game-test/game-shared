namespace MetaFramework.Segmentation
{
    public sealed class Predicate
    {
        public string Field { get; set; } = string.Empty;
        /// <summary>eq | neq | gt | gte | lt | lte | in | not_in | between | exists</summary>
        public string Op { get; set; } = "eq";
        public object? Value { get; set; }
    }
}
