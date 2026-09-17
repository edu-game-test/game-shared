using System;
using System.Collections.Generic;

namespace MetaFramework.FeatureFlags
{
    public sealed class FlagSet
    {
        public int Version { get; set; }
        public List<FlagDefinition> Flags { get; set; } = new();
        public DateTime PublishedAt { get; set; }
        public string PublishedBy { get; set; } = string.Empty;
    }
}
