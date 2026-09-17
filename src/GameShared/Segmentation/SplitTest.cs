using System;
using System.Collections.Generic;

namespace MetaFramework.Segmentation
{
    public sealed class SplitTest
    {
        public string SplitTestId { get; set; } = string.Empty;
        public List<SplitTestVariant> Variants { get; set; } = new();
        public string? TargetingSegment { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
