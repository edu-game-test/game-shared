namespace MetaFramework.FeatureFlags
{
    public interface IFeatureFlagService
    {
        bool IsEnabled(string flagKey);
        int GetInt(string flagKey, int defaultValue = 0);
    }
}
