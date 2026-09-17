using GameShared.Tests.Fakes;
using MetaFramework.FeatureFlags;
using Xunit;

namespace GameShared.Tests.FeatureFlags
{
    public class FlagValueTests
    {
        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void Bool_AsBool(bool input, bool expected) => Assert.Equal(expected, FlagValue.Bool(input).AsBool());

        [Fact]
        public void StringTrue_AsBool_IsTrue() => Assert.True(FlagValue.String("true").AsBool());

        [Fact]
        public void Number_AsInt_Rounds() => Assert.Equal(3, FlagValue.Number(2.6).AsInt(0));

        [Fact]
        public void String_AsInt_Unparseable_ReturnsDefault() => Assert.Equal(7, FlagValue.String("x").AsInt(7));

        [Fact]
        public void Json_AsBool_IsFalse() => Assert.False(FlagValue.Json("{}").AsBool());

        [Fact]
        public void FromObject_ComplexType_BecomesJson()
        {
            var v = FlagValue.FromObject(new { days = 7 }, new SystemTextJsonSerializer());
            Assert.Equal(FlagKind.Json, v.Kind);
            Assert.Equal("{\"days\":7}", v.StringValue);
        }

        [Fact]
        public void FromObject_Int_BecomesNumber() =>
            Assert.Equal(FlagKind.Number, FlagValue.FromObject(5, new SystemTextJsonSerializer()).Kind);
    }
}
