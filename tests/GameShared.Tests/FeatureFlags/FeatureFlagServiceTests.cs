using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GameShared.Tests.Fakes;
using MetaFramework.FeatureFlags;
using Xunit;

namespace GameShared.Tests.FeatureFlags
{
    public class FeatureFlagServiceTests
    {
        private sealed class FakeSource : IFlagSource
        {
            public Dictionary<string, FlagValue> Flags { get; } = new();
            public bool Fail { get; set; }
            public TaskCompletionSource<bool>? Gate { get; set; }
            public int FetchCount { get; private set; }

            public async Task<IReadOnlyDictionary<string, FlagValue>> FetchAsync(CancellationToken ct)
            {
                FetchCount++;
                if (Gate != null) await Gate.Task;
                if (Fail) throw new InvalidOperationException("network down");
                return new Dictionary<string, FlagValue>(Flags);
            }
        }

        private sealed class MemoryCache : IFlagCache
        {
            public IReadOnlyDictionary<string, FlagValue>? Stored { get; set; }
            public IReadOnlyDictionary<string, FlagValue>? Load() => Stored;
            public void Save(IReadOnlyDictionary<string, FlagValue> flags) => Stored = flags;
        }

        private static FeatureFlagService Build(FakeSource source, MemoryCache? cache = null, bool allowOverrides = true, TimeSpan? timeout = null) =>
            new(source, cache, new SystemTextJsonSerializer(), new ListLog(), allowOverrides, timeout);

        [Fact]
        public void BeforeInitialize_ReturnsHardcodedDefaults()
        {
            var svc = Build(new FakeSource());
            Assert.False(svc.IsInitialized);
            Assert.False(svc.IsEnabled("x"));
            Assert.Equal("7day", svc.GetString("variant", "7day"));
            Assert.Equal(2, svc.GetInt("slots", 2));
            Assert.Equal(0.5f, svc.GetFloat("disc", 0.5f));
        }

        [Fact]
        public async Task Initialize_UsesServerValues_AndSavesCache()
        {
            var source = new FakeSource();
            source.Flags["daily_bonus_enabled"] = FlagValue.Bool(true);
            source.Flags["max_event_slots"] = FlagValue.Number(3);
            source.Flags["booster_discount"] = FlagValue.Number(0.25);
            source.Flags["daily_bonus_variant"] = FlagValue.String("premium_7day");
            var cache = new MemoryCache();
            var svc = Build(source, cache);

            await svc.InitializeAsync(CancellationToken.None);

            Assert.True(svc.IsInitialized);
            Assert.True(svc.IsEnabled("daily_bonus_enabled"));
            Assert.Equal(3, svc.GetInt("max_event_slots", 2));
            Assert.Equal(0.25f, svc.GetFloat("booster_discount", 0f));
            Assert.Equal("premium_7day", svc.GetString("daily_bonus_variant", "7day"));
            Assert.NotNull(cache.Stored);
            Assert.Equal(4, cache.Stored!.Count);
        }

        [Fact]
        public async Task FetchFails_WithCache_UsesCachedValues()
        {
            var source = new FakeSource { Fail = true };
            var cache = new MemoryCache { Stored = new Dictionary<string, FlagValue> { ["x"] = FlagValue.Bool(true) } };
            var svc = Build(source, cache);

            await svc.InitializeAsync(CancellationToken.None);

            Assert.True(svc.IsEnabled("x"));
            Assert.False(svc.IsInitialized);
        }

        [Fact]
        public async Task FetchFails_NoCache_UsesDefaults()
        {
            var svc = Build(new FakeSource { Fail = true });
            await svc.InitializeAsync(CancellationToken.None);
            Assert.False(svc.IsEnabled("x"));
            Assert.Equal(9, svc.GetInt("y", 9));
        }

        [Fact]
        public async Task Initialize_TimesOut_ThenAppliesLateResult()
        {
            var source = new FakeSource { Gate = new TaskCompletionSource<bool>() };
            source.Flags["x"] = FlagValue.Bool(true);
            var svc = Build(source, timeout: TimeSpan.FromMilliseconds(50));
            var refreshed = 0;
            svc.OnFlagsRefreshed += () => refreshed++;

            await svc.InitializeAsync(CancellationToken.None);
            Assert.False(svc.IsEnabled("x"));

            source.Gate.SetResult(true);
            await Task.Delay(50);

            Assert.True(svc.IsEnabled("x"));
            Assert.Equal(1, refreshed);
        }

        [Fact]
        public async Task LocalOverride_BeatsServerValue()
        {
            var source = new FakeSource();
            source.Flags["x"] = FlagValue.Bool(false);
            var svc = Build(source);
            await svc.InitializeAsync(CancellationToken.None);

            svc.SetLocalOverride("x", true);
            Assert.True(svc.IsEnabled("x"));

            svc.ClearLocalOverride("x");
            Assert.False(svc.IsEnabled("x"));
        }

        [Fact]
        public void LocalOverride_Disallowed_Throws()
        {
            var svc = Build(new FakeSource(), allowOverrides: false);
            Assert.Throws<NotSupportedException>(() => svc.SetLocalOverride("x", true));
        }

        [Fact]
        public async Task GetJson_DeserializesTypedConfig()
        {
            var source = new FakeSource();
            source.Flags["cfg"] = FlagValue.Json("{\"type\":\"calendar\",\"days\":7,\"loop\":true}");
            var svc = Build(source);
            await svc.InitializeAsync(CancellationToken.None);

            var cfg = svc.GetJson<DailyBonusConfig>("cfg", new DailyBonusConfig());

            Assert.Equal("calendar", cfg.Type);
            Assert.Equal(7, cfg.Days);
            Assert.True(cfg.Loop);
        }

        [Fact]
        public async Task Refresh_FiresEvent_AndReplacesMap()
        {
            var source = new FakeSource();
            source.Flags["x"] = FlagValue.Bool(false);
            var svc = Build(source);
            await svc.InitializeAsync(CancellationToken.None);
            var fired = 0;
            svc.OnFlagsRefreshed += () => fired++;

            source.Flags["x"] = FlagValue.Bool(true);
            await svc.RefreshAsync(CancellationToken.None);

            Assert.True(svc.IsEnabled("x"));
            Assert.Equal(1, fired);
        }

        public sealed class DailyBonusConfig
        {
            public string Type { get; set; } = "";
            public int Days { get; set; }
            public bool Loop { get; set; }
        }
    }
}
