using System;
using System.Collections.Generic;

namespace MetaFramework.FeatureFlags
{
    /// <summary>Server-side flag definition (feature-flag-system.md "Flag configuration schema").</summary>
    public sealed class FlagDefinition
    {
        public string Key { get; set; } = string.Empty;
        public FlagType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        /// <summary>draft | development | qa | soft_launch | gradual_rollout | full_rollout | retired</summary>
        public string Lifecycle { get; set; } = "draft";
        public object? DefaultValue { get; set; }
        public List<FlagSegmentValue> SegmentValues { get; set; } = new();
        public FlagRollout? Rollout { get; set; }
        public Dictionary<string, object?> PlatformOverrides { get; set; } = new();
        /// <summary>Semver floor; players on older clients get DefaultValue.</summary>
        public string? MinClientVersion { get; set; }
        /// <summary>Push a flagsChanged event on change (kill-switch).</summary>
        public bool Push { get; set; }
        /// <summary>Send resolved value to analytics as a player property.</summary>
        public bool Track { get; set; }
    }
}
