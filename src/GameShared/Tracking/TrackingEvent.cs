using System;
using System.Collections.Generic;

namespace MetaFramework.Tracking
{
    /// <summary>One analytics event. Name is snake_case; properties are primitives or strings.</summary>
    public sealed class TrackingEvent
    {
        private readonly Dictionary<string, object> _properties = new();

        public TrackingEvent(string name) { Name = name; }

        public string Name { get; }
        public IReadOnlyDictionary<string, object> Properties => _properties;
        /// <summary>Set by the tracking service when the event is enqueued.</summary>
        public DateTime? ClientTimestamp { get; set; }

        public TrackingEvent With(string key, object value)
        {
            _properties[key] = value;
            return this;
        }
    }
}
