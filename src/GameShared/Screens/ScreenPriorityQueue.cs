using System.Collections.Generic;
using System.Linq;
using MetaFramework.Common;

namespace MetaFramework.Screens
{
    /// <summary>
    /// Pending screen requests ordered by (Priority asc, arrival asc). Duplicates dropped unless allowed;
    /// expired requests are skipped at dequeue (screen-system.md "Queue Rules"). Not thread-safe (Unity main thread).
    /// </summary>
    public sealed class ScreenPriorityQueue
    {
        private readonly IClock _clock;
        private readonly List<(long Sequence, ScreenRequest Request)> _items = new();
        private long _sequence;

        public ScreenPriorityQueue(IClock clock) => _clock = clock;

        public int Count => _items.Count;

        public bool Contains(string screenId) => _items.Any(x => x.Request.ScreenId == screenId);

        /// <returns>false if the request was dropped as a duplicate.</returns>
        public bool Enqueue(ScreenRequest request, bool allowDuplicate = false)
        {
            if (!allowDuplicate && Contains(request.ScreenId))
                return false;
            _items.Add((_sequence++, request));
            return true;
        }

        public bool Cancel(string screenId) => _items.RemoveAll(x => x.Request.ScreenId == screenId) > 0;

        /// <summary>Removes and returns the most urgent non-expired request, or null when empty.</summary>
        public ScreenRequest? Dequeue()
        {
            var now = _clock.UtcNow;
            _items.RemoveAll(x => x.Request.ExpiresAt.HasValue && x.Request.ExpiresAt.Value <= now);
            if (_items.Count == 0)
                return null;

            var best = _items.OrderBy(x => x.Request.Priority).ThenBy(x => x.Sequence).First();
            _items.Remove(best);
            return best.Request;
        }
    }
}
