using System;
using MetaFramework.Common;
using MetaFramework.Segmentation;
using Xunit;

namespace GameShared.Tests.Segmentation
{
    public class SplitTestResolverTests
    {
        private static readonly DateTime Now = new(2026, 9, 10, 12, 0, 0, DateTimeKind.Utc);

        private static SplitTest Test() => new()
        {
            SplitTestId = "map_scroll_speed",
            StartDate = Now.AddDays(-1),
            Variants =
            {
                new SplitTestVariant { Name = "control", BucketRange = new[] { 0, 49 } },
                new SplitTestVariant { Name = "fast_scroll", BucketRange = new[] { 50, 79 } },
                new SplitTestVariant { Name = "slow_scroll", BucketRange = new[] { 80, 99 } },
            },
        };

        [Fact]
        public void Variant_MatchesBucket()
        {
            var bucket = StableBucket.Compute("U1", "map_scroll_speed");
            var expected = bucket < 50 ? "control" : bucket < 80 ? "fast_scroll" : "slow_scroll";
            Assert.Equal(expected, SplitTestResolver.Resolve(Test(), "U1", _ => true, Now));
        }

        [Fact]
        public void OutsideTargetingSegment_IsControl()
        {
            var t = Test(); t.TargetingSegment = "new_player";
            Assert.Equal("control", SplitTestResolver.Resolve(t, "U1", _ => false, Now));
        }

        [Fact]
        public void BeforeStart_OrAfterEnd_IsControl()
        {
            var t = Test();
            Assert.Equal("control", SplitTestResolver.Resolve(t, "U1", _ => true, t.StartDate.AddHours(-1)));
            t.EndDate = Now.AddHours(-1);
            Assert.Equal("control", SplitTestResolver.Resolve(t, "U1", _ => true, Now));
        }
    }
}
