using System;
using System.Collections.Generic;

namespace MetaFramework.Contract
{
    /// <summary>Returned by the game when a level ends. The meta layer reads it to grant rewards and update progression.</summary>
    public interface ILevelResult
    {
        int LevelNumber { get; }
        /// <summary>Echo of LevelLaunchContext.LevelSessionId so the server can match the attempt (Decision D20).</summary>
        string LevelSessionId { get; }
        LevelOutcome Outcome { get; }
        long Score { get; }
        int MovesUsed { get; }
        int MovesRemaining { get; }
        /// <summary>0–3 (0 on a loss).</summary>
        int StarsEarned { get; }
        TimeSpan PlayDuration { get; }
        IReadOnlyList<CollectedItem> ItemsCollected { get; }
        IReadOnlyDictionary<string, int> CustomMetrics { get; }
        /// <summary>True if the player was close to winning (drives the Extra Moves offer).</summary>
        bool IsCloseCall { get; }
        /// <summary>0.0–1.0 fraction of objectives cleared.</summary>
        float PercentObjectiveCompleted { get; }
    }
}
