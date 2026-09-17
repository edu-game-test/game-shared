using System.Threading.Tasks;

namespace MetaFramework.Contract
{
    /// <summary>Implemented by the game. The meta layer calls it when the player taps Play.</summary>
    public interface ILevelProvider
    {
        /// <summary>Loads the game scene and starts gameplay. Must call callback.OnLevelEnded exactly once.</summary>
        Task LaunchLevelAsync(LevelLaunchContext context, ILevelResultCallback callback);

        /// <summary>True while a level is active (guards accidental double-launch).</summary>
        bool IsLevelActive { get; }
    }
}
