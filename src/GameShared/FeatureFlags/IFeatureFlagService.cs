using System;

namespace MetaFramework.FeatureFlags
{
    /// <summary>Read access to flags resolved for the current player. All reads are synchronous O(1) (feature-flag-system.md).</summary>
    public interface IFeatureFlagService
    {
        bool IsEnabled(string flagKey);
        string GetString(string flagKey, string defaultValue = "");
        int GetInt(string flagKey, int defaultValue = 0);
        float GetFloat(string flagKey, float defaultValue = 0f);
        T GetJson<T>(string flagKey, T defaultValue = default!);

        /// <summary>Dev/QA only. Throws NotSupportedException in production builds.</summary>
        void SetLocalOverride(string flagKey, object value);
        void ClearLocalOverride(string flagKey);
        void ClearAllLocalOverrides();

        /// <summary>True once flags have been fetched from the server at least once this session.</summary>
        bool IsInitialized { get; }
        event Action? OnFlagsRefreshed;
    }
}
