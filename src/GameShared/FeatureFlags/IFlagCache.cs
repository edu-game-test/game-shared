using System.Collections.Generic;

namespace MetaFramework.FeatureFlags
{
    /// <summary>Persists the last successfully fetched flag map (PlayerPrefs on mobile, JSON file on desktop/web).</summary>
    public interface IFlagCache
    {
        IReadOnlyDictionary<string, FlagValue>? Load();
        void Save(IReadOnlyDictionary<string, FlagValue> flags);
    }
}
