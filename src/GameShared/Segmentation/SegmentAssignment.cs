using System;
using System.Collections.Generic;

namespace MetaFramework.Segmentation
{
    public sealed class SegmentAssignment
    {
        public List<string> SegmentIds { get; set; } = new();
        public Dictionary<string, string> AbAssignments { get; set; } = new();
        public DateTime EvaluatedAt { get; set; }
        /// <summary>Null when no segment carries a TTL.</summary>
        public DateTime? ExpiresAt { get; set; }
        public int SegmentSetVersion { get; set; }
    }
}
