using System;
using System.Linq;
using MetaFramework.Common;

namespace MetaFramework.Segmentation
{
    /// <summary>Variant resolution (segmentation-system.md "A/B Testing Support"): targeting → dates → bucket range; else "control".</summary>
    public static class SplitTestResolver
    {
        public const string Control = "control";

        public static string Resolve(SplitTest test, string userId, Func<string, bool> isInSegment, DateTime nowUtc)
        {
            if (!string.IsNullOrEmpty(test.TargetingSegment) && !isInSegment(test.TargetingSegment!))
                return Control;
            if (nowUtc < test.StartDate) return Control;
            if (test.EndDate.HasValue && nowUtc > test.EndDate.Value) return Control;

            var bucket = StableBucket.Compute(userId, test.SplitTestId);
            var variant = test.Variants.FirstOrDefault(v => v.BucketRange.Length == 2 && bucket >= v.BucketRange[0] && bucket <= v.BucketRange[1]);
            return variant?.Name ?? Control;
        }
    }
}
