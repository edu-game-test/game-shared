using System.Collections.Generic;
using MetaFramework.Segmentation;
using Xunit;

namespace GameShared.Tests.Segmentation
{
    public class PredicateEvaluatorTests
    {
        [Theory]
        [InlineData("eq", 42, 42, true)]
        [InlineData("eq", 42, 41, false)]
        [InlineData("neq", 42, 41, true)]
        [InlineData("gt", 42, 41, true)]
        [InlineData("gte", 42, 42, true)]
        [InlineData("lt", 42, 43, true)]
        [InlineData("lte", 42, 41, false)]
        public void NumericOps(string op, object field, object value, bool expected) =>
            Assert.Equal(expected, PredicateEvaluator.Matches(new Predicate { Op = op, Value = value }, field));

        [Fact]
        public void Eq_LongVsDouble_AreEqual() =>
            Assert.True(PredicateEvaluator.Matches(new Predicate { Op = "eq", Value = 20.0 }, 20L));

        [Fact]
        public void In_StringList()
        {
            var p = new Predicate { Op = "in", Value = new List<object> { "android", "ios" } };
            Assert.True(PredicateEvaluator.Matches(p, "ios"));
            Assert.False(PredicateEvaluator.Matches(p, "web"));
        }

        [Fact]
        public void NotIn_NullField_IsTrue() =>
            Assert.True(PredicateEvaluator.Matches(new Predicate { Op = "not_in", Value = new List<object> { "x" } }, null));

        [Fact]
        public void Between_Inclusive()
        {
            var p = new Predicate { Op = "between", Value = new List<object> { 5, 50 } };
            Assert.True(PredicateEvaluator.Matches(p, 5));
            Assert.True(PredicateEvaluator.Matches(p, 50));
            Assert.False(PredicateEvaluator.Matches(p, 51));
        }

        [Fact]
        public void Exists()
        {
            Assert.True(PredicateEvaluator.Matches(new Predicate { Op = "exists" }, "US"));
            Assert.False(PredicateEvaluator.Matches(new Predicate { Op = "exists" }, null));
        }

        [Fact]
        public void MatchesAll_And_Or()
        {
            var def = new SegmentDefinition
            {
                SegmentId = "high_value_payer",
                Predicates =
                {
                    new Predicate { Field = SegmentFields.LifetimeSpendUsd, Op = "gte", Value = 20.0 },
                    new Predicate { Field = SegmentFields.Platform, Op = "in", Value = new List<object> { "android", "ios" } },
                },
            };
            object? Resolve(string f) => f == SegmentFields.LifetimeSpendUsd ? (object?)25.0 : "web";

            Assert.False(PredicateEvaluator.MatchesAll(def, Resolve));
            def.PredicateLogic = PredicateLogic.Or;
            Assert.True(PredicateEvaluator.MatchesAll(def, Resolve));
        }

        [Fact]
        public void NoPredicates_NeverMatches() =>
            Assert.False(PredicateEvaluator.MatchesAll(new SegmentDefinition(), _ => 1));
    }
}
