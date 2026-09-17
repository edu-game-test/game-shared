using System;

namespace MetaFramework.Screens
{
    public sealed class ScreenRequest
    {
        public string ScreenId { get; init; } = string.Empty;
        public int Priority { get; init; }
        public ScreenContext? Context { get; init; }
        public Action? OnClosed { get; init; }
        /// <summary>Dropped if not shown by this time.</summary>
        public DateTime? ExpiresAt { get; init; }
    }
}
