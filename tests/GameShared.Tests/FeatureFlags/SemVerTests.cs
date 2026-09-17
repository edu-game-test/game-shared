using MetaFramework.FeatureFlags;
using Xunit;

namespace GameShared.Tests.FeatureFlags
{
    public class SemVerTests
    {
        [Theory]
        [InlineData("1.4.2", "1.5.0", -1)]
        [InlineData("1.5.0", "1.5.0", 0)]
        [InlineData("2.0.0", "1.9.9", 1)]
        [InlineData("1.10.0", "1.9.0", 1)]
        [InlineData("1.4.2-beta", "1.4.2", 0)]
        public void Compare(string a, string b, int expectedSign) =>
            Assert.Equal(expectedSign, System.Math.Sign(SemVer.Compare(a, b)));
    }
}
