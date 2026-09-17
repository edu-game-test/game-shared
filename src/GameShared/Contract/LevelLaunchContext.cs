using System.Collections.Generic;

namespace MetaFramework.Contract
{
    public sealed class LevelLaunchContext
    {
        public int LevelNumber { get; init; }
        /// <summary>Server-issued attempt id from POST /api/v1/levels/{n}/start (Decision D20). Empty in offline/debug launches.</summary>
        public string LevelSessionId { get; init; } = string.Empty;
        /// <summary>Key into the level config CDN bundle.</summary>
        public string LevelConfigKey { get; init; } = string.Empty;
        /// <summary>Raw LevelConfig JSON (levels-system.md schema). Passed through opaquely; the game parses game_data.</summary>
        public string LevelConfigJson { get; init; } = string.Empty;
        public IReadOnlyDictionary<string, string> ServerParams { get; init; } = new Dictionary<string, string>();
        public IReadOnlyList<string> ActiveBoosterIds { get; init; } = new List<string>();
        public bool IsRetry { get; init; }

        // AI System hooks — Day 1 neutral defaults, Phase 2 computed (ai-system.md "DDA Extension").
        /// <summary>-1.0 (easier) … +1.0 (harder); 0.0 = no adjustment.</summary>
        public float DifficultyBias { get; init; } = 0f;
        /// <summary>Booster to offer if the player is struggling; null when DDA has no suggestion.</summary>
        public string? BoosterSuggestion { get; init; } = null;
    }
}
