using System;
using MetaFramework.Common;
using Xunit;

namespace GameShared.Tests
{
    public class SystemClockTests
    {
        [Fact]
        public void UtcNow_IsUtcKind()
        {
            IClock clock = new SystemClock();
            Assert.Equal(DateTimeKind.Utc, clock.UtcNow.Kind);
        }
    }
}
