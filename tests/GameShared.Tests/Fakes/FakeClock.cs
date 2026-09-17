using System;
using MetaFramework.Common;

namespace GameShared.Tests.Fakes
{
    public sealed class FakeClock : IClock
    {
        public FakeClock(DateTime utcNow) => UtcNow = utcNow;
        public DateTime UtcNow { get; set; }
        public void Advance(TimeSpan by) => UtcNow += by;
    }
}
