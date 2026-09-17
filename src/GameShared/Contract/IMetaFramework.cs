using MetaFramework.FeatureFlags;
using MetaFramework.Tracking;

namespace MetaFramework.Contract
{
    /// <summary>Handed to the game in IGamePlugin.OnMetaReady. The only surface the game layer sees.</summary>
    public interface IMetaFramework
    {
        void RegisterLevelProvider(ILevelProvider provider);
        void RegisterEconomy(IGameEconomy economy);
        IFeatureFlagService FeatureFlags { get; }
        ITrackingService Tracking { get; }
        ICurrencyCollectedCallback CurrencyCollected { get; }
    }
}
