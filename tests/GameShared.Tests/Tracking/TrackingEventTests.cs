using MetaFramework.Tracking;
using Xunit;

namespace GameShared.Tests.Tracking
{
    public class TrackingEventTests
    {
        [Fact]
        public void With_IsFluent_AndStoresProperties()
        {
            var evt = new TrackingEvent("daily_bonus_claimed").With("streak_days", 4).With("placement", "DAILY_BONUS");
            Assert.Equal("daily_bonus_claimed", evt.Name);
            Assert.Equal(4, evt.Properties["streak_days"]);
            Assert.Equal("DAILY_BONUS", evt.Properties["placement"]);
        }

        [Fact]
        public void With_SameKeyTwice_LastWins()
        {
            var evt = new TrackingEvent("x").With("k", 1).With("k", 2);
            Assert.Equal(2, evt.Properties["k"]);
        }
    }
}
