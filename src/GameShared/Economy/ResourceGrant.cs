namespace MetaFramework.Economy
{
    /// <summary>One line of a batch grant / store product contents.</summary>
    public sealed class ResourceGrant
    {
        public ResourceGrant() { }
        public ResourceGrant(string resourceId, int amount) { ResourceId = resourceId; Amount = amount; }
        public string ResourceId { get; init; } = string.Empty;
        public int Amount { get; init; }
    }
}
