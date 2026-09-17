using System;
using System.Collections.Generic;

namespace MetaFramework.FeatureFlags
{
    /// <summary>
    /// Wire shape of GET /api/v1/flags. Values are native JSON (bool/string/number/object).
    /// The server builds this from resolved values; the client adapter converts each entry with FlagValue.FromObject
    /// after its JSON library has produced primitive CLR values (Newtonsoft: JValue.Value / JObject.ToString()).
    /// </summary>
    public sealed class FeatureFlagsResponse
    {
        public int SchemaVersion { get; set; } = 1;
        public DateTime GeneratedAt { get; set; }
        public Dictionary<string, object> Flags { get; set; } = new();
    }
}
