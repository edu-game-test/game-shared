using MetaFramework.Common;
using Xunit;

namespace GameShared.Tests.Common
{
    public class StableBucketTests
    {
        [Fact]
        public void SameInput_SameBucket() =>
            Assert.Equal(StableBucket.Compute("U123", "daily_bonus"), StableBucket.Compute("U123", "daily_bonus"));

        [Fact]
        public void AlwaysWithinRange()
        {
            for (int i = 0; i < 1000; i++)
            {
                var b = StableBucket.Compute("user" + i, "flag");
                Assert.InRange(b, 0, 99);
            }
        }

        [Fact]
        public void DifferentKeys_GiveDifferentDistribution()
        {
            int same = 0;
            for (int i = 0; i < 200; i++)
                if (StableBucket.Compute("user" + i, "a") == StableBucket.Compute("user" + i, "b")) same++;
            Assert.True(same < 40, $"too many collisions: {same}");
        }

        [Fact]
        public void KnownVector_MatchesReferenceImplementation()
        {
            // Reference: SHA256("u1:k1") as big-endian integer mod 100. Computed once with System.Numerics.BigInteger:
            // new BigInteger(SHA256("u1:k1").Reverse().Concat(new byte[]{0}).ToArray()) % 100 == 20
            Assert.Equal(20, StableBucket.Compute("u1", "k1"));
        }
    }
}
