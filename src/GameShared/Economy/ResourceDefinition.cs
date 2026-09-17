namespace MetaFramework.Economy
{
    /// <summary>Registered schema for one resource type (economy-system.md "ResourceDefinition Schema").</summary>
    public sealed class ResourceDefinition
    {
        /// <summary>Unique within the game namespace, e.g. "coins", "lives", "booster_hammer".</summary>
        public string Id { get; init; } = string.Empty;
        /// <summary>Localization key.</summary>
        public string DisplayName { get; init; } = string.Empty;
        /// <summary>Addressable asset key for the icon sprite.</summary>
        public string IconAssetKey { get; init; } = string.Empty;
        public ResourceType Type { get; init; }
        /// <summary>null = no cap.</summary>
        public int? Cap { get; init; }
        public bool OverflowAllowed { get; init; }
        public CapExceededPolicy CapPolicy { get; init; } = CapExceededPolicy.Clamp;
        /// <summary>null = not regenerating.</summary>
        public RegenerationConfig? Regeneration { get; init; }
        /// <summary>Hard-currency guard: client has no code path to add it locally.</summary>
        public bool ServerAuthoritative { get; init; }
        public bool ShowInHud { get; init; }
        /// <summary>Remote config key overriding Cap, e.g. "economy.lives.cap".</summary>
        public string? CapConfigKey { get; init; }
        /// <summary>Remote config key overriding Regeneration.IntervalSeconds.</summary>
        public string? RegenConfigKey { get; init; }
    }
}
