using System.Collections.Generic;
using System.Linq;
using MetaFramework.Common;

namespace MetaFramework.FeatureFlags
{
    public sealed class FlagEvaluationInput
    {
        public string UserId { get; init; } = string.Empty;
        public string Platform { get; init; } = string.Empty;
        public string ClientVersion { get; init; } = "0.0.0";
        /// <summary>Segments the player belongs to.</summary>
        public IReadOnlyCollection<string> Segments { get; init; } = new List<string>();
        /// <summary>Per-user overrides set by CS/QA tooling.</summary>
        public IReadOnlyDictionary<string, object?>? UserOverrides { get; init; }
    }

    /// <summary>
    /// Server-side resolution (feature-flag-system.md "Flag Evaluation Logic"):
    /// user override → platform override → highest-priority matching segment value → rollout → default.
    /// Plan decision: platform overrides sit directly below user overrides.
    /// </summary>
    public static class FlagEvaluator
    {
        public static object? Resolve(FlagDefinition flag, FlagEvaluationInput input)
        {
            if (input.UserOverrides != null && input.UserOverrides.TryGetValue(flag.Key, out var userValue))
                return userValue;

            if (!string.IsNullOrEmpty(flag.MinClientVersion) && SemVer.Compare(input.ClientVersion, flag.MinClientVersion!) < 0)
                return flag.DefaultValue;

            if (flag.PlatformOverrides.TryGetValue(input.Platform, out var platformValue))
                return platformValue;

            var segmentMatch = flag.SegmentValues
                .Where(sv => input.Segments.Contains(sv.Segment))
                .OrderByDescending(sv => sv.Priority)
                .FirstOrDefault();
            if (segmentMatch != null)
                return segmentMatch.Value;

            if (flag.Rollout != null && flag.Rollout.Percentage > 0)
            {
                var bucket = StableBucket.Compute(input.UserId, flag.Key);
                return bucket < flag.Rollout.Percentage ? flag.Rollout.EnabledValue : (flag.Rollout.DisabledValue ?? flag.DefaultValue);
            }

            return flag.DefaultValue;
        }

        /// <summary>Resolves every flag; flags resolving to null are omitted so the client falls back to its hardcoded default.</summary>
        public static Dictionary<string, object> ResolveAll(FlagSet set, FlagEvaluationInput input)
        {
            var result = new Dictionary<string, object>();
            foreach (var flag in set.Flags)
            {
                if (flag.Lifecycle == "retired") continue;
                var value = Resolve(flag, input);
                if (value != null) result[flag.Key] = value;
            }
            return result;
        }
    }
}
