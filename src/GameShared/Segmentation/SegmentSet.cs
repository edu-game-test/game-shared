using System;
using System.Collections.Generic;

namespace MetaFramework.Segmentation
{
    public sealed class SegmentSet
    {
        public int Version { get; set; }
        public List<SegmentDefinition> Segments { get; set; } = new();
        public List<SplitTest> SplitTests { get; set; } = new();
        public DateTime PublishedAt { get; set; }
        public string PublishedBy { get; set; } = string.Empty;
    }
}
