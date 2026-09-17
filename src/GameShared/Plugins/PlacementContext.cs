using System.Collections.Generic;

namespace MetaFramework.Plugins
{
    public sealed class PlacementContext
    {
        public PlacementContext(string placement, IScreenContext? screen = null,
            IReadOnlyDictionary<string, object>? data = null)
        {
            Placement = placement;
            Screen = screen;
            Data = data ?? new Dictionary<string, object>();
        }

        public string Placement { get; }
        /// <summary>Null for logical placements that have no screen (e.g. REWARD_POST_COLLECT).</summary>
        public IScreenContext? Screen { get; }
        /// <summary>Bag of placement-specific data, e.g. POST_LEVEL_WIN: level_id, stars, score.</summary>
        public IReadOnlyDictionary<string, object> Data { get; }

        public T? Get<T>(string key) => Data.TryGetValue(key, out var value) && value is T typed ? typed : default;
    }
}
