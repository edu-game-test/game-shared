using System;
using GameShared.Tests.Fakes;
using MetaFramework.Screens;
using Xunit;

namespace GameShared.Tests.Screens
{
    public class ScreenPriorityQueueTests
    {
        private static readonly DateTime Now = new(2026, 9, 10, 12, 0, 0, DateTimeKind.Utc);
        private static ScreenRequest Req(string id, int prio, DateTime? expires = null) =>
            new() { ScreenId = id, Priority = prio, ExpiresAt = expires };

        [Fact]
        public void Dequeue_LowestPriorityNumberFirst()
        {
            var q = new ScreenPriorityQueue(new FakeClock(Now));
            q.Enqueue(Req("EventPopup", ScreenPriorities.EventPopup));
            q.Enqueue(Req("LevelWinScreen", ScreenPriorities.LevelResult));
            q.Enqueue(Req("DailyBonusScreen", ScreenPriorities.DailyLogin));

            Assert.Equal("LevelWinScreen", q.Dequeue()!.ScreenId);
            Assert.Equal("DailyBonusScreen", q.Dequeue()!.ScreenId);
            Assert.Equal("EventPopup", q.Dequeue()!.ScreenId);
            Assert.Null(q.Dequeue());
        }

        [Fact]
        public void SamePriority_IsFifo()
        {
            var q = new ScreenPriorityQueue(new FakeClock(Now));
            q.Enqueue(Req("A", 40));
            q.Enqueue(Req("B", 40));
            q.Enqueue(Req("C", 40));
            Assert.Equal("A", q.Dequeue()!.ScreenId);
            Assert.Equal("B", q.Dequeue()!.ScreenId);
            Assert.Equal("C", q.Dequeue()!.ScreenId);
        }

        [Fact]
        public void Duplicate_IsDroppedByDefault()
        {
            var q = new ScreenPriorityQueue(new FakeClock(Now));
            Assert.True(q.Enqueue(Req("A", 40)));
            Assert.False(q.Enqueue(Req("A", 10)));
            Assert.Equal(1, q.Count);
        }

        [Fact]
        public void Duplicate_AllowedWhenOptedIn()
        {
            var q = new ScreenPriorityQueue(new FakeClock(Now));
            q.Enqueue(Req("A", 40));
            Assert.True(q.Enqueue(Req("A", 40), allowDuplicate: true));
            Assert.Equal(2, q.Count);
        }

        [Fact]
        public void Cancel_RemovesPending()
        {
            var q = new ScreenPriorityQueue(new FakeClock(Now));
            q.Enqueue(Req("A", 40));
            Assert.True(q.Cancel("A"));
            Assert.False(q.Cancel("A"));
            Assert.False(q.Contains("A"));
        }

        [Fact]
        public void Expired_IsSkippedOnDequeue()
        {
            var clock = new FakeClock(Now);
            var q = new ScreenPriorityQueue(clock);
            q.Enqueue(Req("Stale", 10, expires: Now.AddMinutes(1)));
            q.Enqueue(Req("Fresh", 50));
            clock.Advance(TimeSpan.FromMinutes(2));

            Assert.Equal("Fresh", q.Dequeue()!.ScreenId);
            Assert.Equal(0, q.Count);
        }
    }
}
