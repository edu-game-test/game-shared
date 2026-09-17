using System;
using GameShared.Tests.Fakes;
using Xunit;

namespace GameShared.Tests.Common
{
    public class FakesTests
    {
        [Fact]
        public void FakeClock_Advance_MovesTime()
        {
            var clock = new FakeClock(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            clock.Advance(TimeSpan.FromMinutes(30));
            Assert.Equal(new DateTime(2026, 1, 1, 0, 30, 0, DateTimeKind.Utc), clock.UtcNow);
        }

        [Fact]
        public void Serializer_RoundTrips_CamelCase()
        {
            var json = new SystemTextJsonSerializer();
            var text = json.Serialize(new Sample { Name = "x", Count = 2 });
            Assert.Contains("\"name\":\"x\"", text);
            Assert.Equal(2, json.Deserialize<Sample>(text).Count);
        }

        private sealed class Sample { public string Name { get; set; } = ""; public int Count { get; set; } }
    }
}
