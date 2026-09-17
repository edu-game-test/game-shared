using System.Collections.Generic;
using MetaFramework.FeatureFlags;

namespace GameShared.Tests.Fakes
{
    /// <summary>Dictionary-backed IFeatureFlagService. Extended in Task 4 when the interface grows.</summary>
    public sealed class FakeFeatureFlags : IFeatureFlagService
    {
        public Dictionary<string, object> Values { get; } = new();

        public FakeFeatureFlags Enable(params string[] keys)
        {
            foreach (var k in keys) Values[k] = true;
            return this;
        }

        public bool IsEnabled(string flagKey) => Values.TryGetValue(flagKey, out var v) && v is bool b && b;
        public int GetInt(string flagKey, int defaultValue = 0) => Values.TryGetValue(flagKey, out var v) && v is int i ? i : defaultValue;
    }
}
