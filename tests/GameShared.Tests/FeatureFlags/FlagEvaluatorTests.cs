using System.Collections.Generic;
using MetaFramework.FeatureFlags;
using Xunit;

namespace GameShared.Tests.FeatureFlags
{
    public class FlagEvaluatorTests
    {
        private static FlagDefinition Flag() => new()
        {
            Key = "daily_bonus_variant", Type = FlagType.String, DefaultValue = "7day",
            SegmentValues =
            {
                new FlagSegmentValue { Segment = "high_spenders", Priority = 100, Value = "premium_7day" },
                new FlagSegmentValue { Segment = "ab_daily_bonus_b", Priority = 50, Value = "14day" },
            },
        };

        private static FlagEvaluationInput Input(params string[] segments) =>
            new() { UserId = "U123", Platform = "android", ClientVersion = "1.4.2", Segments = segments };

        [Fact]
        public void Default_WhenNothingMatches() => Assert.Equal("7day", FlagEvaluator.Resolve(Flag(), Input()));

        [Fact]
        public void HighestPrioritySegmentWins() =>
            Assert.Equal("premium_7day", FlagEvaluator.Resolve(Flag(), Input("ab_daily_bonus_b", "high_spenders")));

        [Fact]
        public void LowerPrioritySegment_WhenOnlyItMatches() =>
            Assert.Equal("14day", FlagEvaluator.Resolve(Flag(), Input("ab_daily_bonus_b")));

        [Fact]
        public void UserOverride_BeatsEverything()
        {
            var input = new FlagEvaluationInput
            {
                UserId = "U123", Platform = "android", Segments = new[] { "high_spenders" },
                UserOverrides = new Dictionary<string, object?> { ["daily_bonus_variant"] = "qa_variant" },
            };
            Assert.Equal("qa_variant", FlagEvaluator.Resolve(Flag(), input));
        }

        [Fact]
        public void PlatformOverride_BeatsSegment()
        {
            var flag = Flag();
            flag.PlatformOverrides["web"] = "web_variant";
            var input = new FlagEvaluationInput { UserId = "U1", Platform = "web", Segments = new[] { "high_spenders" } };
            Assert.Equal("web_variant", FlagEvaluator.Resolve(flag, input));
        }

        [Fact]
        public void MinClientVersion_TooOld_ReturnsDefault()
        {
            var flag = Flag();
            flag.MinClientVersion = "1.5.0";
            Assert.Equal("7day", FlagEvaluator.Resolve(flag, Input("high_spenders")));
        }

        [Fact]
        public void Rollout_IsDeterministic_AndRespectsPercentage()
        {
            var flag = new FlagDefinition
            {
                Key = "events_enabled", Type = FlagType.Boolean, DefaultValue = false,
                Rollout = new FlagRollout { Percentage = 50, EnabledValue = true, DisabledValue = false },
            };
            int enabled = 0;
            for (int i = 0; i < 1000; i++)
            {
                var v = (bool)FlagEvaluator.Resolve(flag, new FlagEvaluationInput { UserId = "user" + i, Platform = "ios" })!;
                var again = (bool)FlagEvaluator.Resolve(flag, new FlagEvaluationInput { UserId = "user" + i, Platform = "ios" })!;
                Assert.Equal(v, again);
                if (v) enabled++;
            }
            Assert.InRange(enabled, 400, 600);
        }

        [Fact]
        public void ResolveAll_SkipsRetired_AndNullValues()
        {
            var set = new FlagSet
            {
                Flags =
                {
                    new FlagDefinition { Key = "a", DefaultValue = true },
                    new FlagDefinition { Key = "b", DefaultValue = true, Lifecycle = "retired" },
                    new FlagDefinition { Key = "c", DefaultValue = null },
                },
            };
            var all = FlagEvaluator.ResolveAll(set, Input());
            Assert.Single(all);
            Assert.True((bool)all["a"]);
        }
    }
}
