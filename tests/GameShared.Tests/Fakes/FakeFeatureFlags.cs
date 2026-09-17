using System.Collections.Generic;
using MetaFramework.FeatureFlags;

namespace GameShared.Tests.Fakes
{
    /// <summary>Dictionary-backed IFeatureFlagService for tests.</summary>
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
        public string GetString(string flagKey, string defaultValue = "") => Values.TryGetValue(flagKey, out var v) && v is string s ? s : defaultValue;
        public float GetFloat(string flagKey, float defaultValue = 0f) => Values.TryGetValue(flagKey, out var v) && v is float f ? f : defaultValue;
        public T GetJson<T>(string flagKey, T defaultValue = default!) => Values.TryGetValue(flagKey, out var v) && v is T t ? t : defaultValue;
        public void SetLocalOverride(string flagKey, object value) => Values[flagKey] = value;
        public void ClearLocalOverride(string flagKey) => Values.Remove(flagKey);
        public void ClearAllLocalOverrides() => Values.Clear();
        public bool IsInitialized => true;
        public event System.Action? OnFlagsRefreshed;
        public void RaiseRefreshed() => OnFlagsRefreshed?.Invoke();
    }
}
