using System;
using System.Collections.Generic;
using System.Linq;
using GameShared.Tests.Fakes;
using MetaFramework.Plugins;
using Xunit;

namespace GameShared.Tests.Plugins
{
    public class PluginManagerTests
    {
        private sealed class RecordingPlugin : PluginBase
        {
            private readonly (string placement, int priority)[] _regs;
            private readonly bool _claimsPopup;
            public List<string> Calls { get; } = new();
            public RecordingPlugin(string id, bool claimsPopup, params (string, int)[] regs)
            { PluginId = id; _claimsPopup = claimsPopup; _regs = regs; }
            public override string PluginId { get; }
            public override void Register(IPluginRegistry registry)
            { foreach (var (p, prio) in _regs) registry.RegisterPlacement(p, prio); }
            public override bool OnShowPopup(PlacementContext ctx) { Calls.Add("popup"); return _claimsPopup; }
            public override void OnInit(PlacementContext ctx) => Calls.Add("init");
            public override void OnButtonPressed(PlacementContext ctx, string buttonName) => Calls.Add("button:" + buttonName);
        }

        private sealed class ThrowingPlugin : PluginBase
        {
            public override string PluginId => "thrower";
            public override void Register(IPluginRegistry registry) => registry.RegisterPlacement(Placements.MapScreen, 10);
            public override void OnInit(PlacementContext ctx) => throw new InvalidOperationException("boom");
        }

        private static PlacementContext Ctx(string placement) => new(placement);

        [Fact]
        public void DisabledByFlag_IsNeverRegistered()
        {
            var flags = new FakeFeatureFlags();
            var plugin = new RecordingPlugin("a", false, (Placements.MapScreen, 100));
            var mgr = new PluginManager(flags, new ListLog(), new[] { plugin });

            mgr.Invoke(Placements.MapScreen, Ctx(Placements.MapScreen), PluginHook.Init);

            Assert.Empty(plugin.Calls);
            Assert.Empty(mgr.GetPlugins(Placements.MapScreen));
        }

        [Fact]
        public void Init_CallsAllPlugins_InPriorityOrder()
        {
            var flags = new FakeFeatureFlags().Enable("a", "b", "c");
            var a = new RecordingPlugin("a", false, (Placements.MapScreen, 300));
            var b = new RecordingPlugin("b", false, (Placements.MapScreen, 100));
            var c = new RecordingPlugin("c", false, (Placements.MapScreen, 200));
            var mgr = new PluginManager(flags, new ListLog(), new IPlugin[] { a, b, c });

            var ordered = mgr.GetPlugins(Placements.MapScreen).Select(p => p.PluginId).ToArray();

            Assert.Equal(new[] { "b", "c", "a" }, ordered);
        }

        [Fact]
        public void ShowPopup_FirstClaimerWins_LowerPriorityNotCalled()
        {
            var flags = new FakeFeatureFlags().Enable("streak", "daily", "event");
            var streak = new RecordingPlugin("streak", false, (Placements.MapScreen, 100));
            var daily  = new RecordingPlugin("daily",  true,  (Placements.MapScreen, 150));
            var evt    = new RecordingPlugin("event",  true,  (Placements.MapScreen, 200));
            var mgr = new PluginManager(flags, new ListLog(), new IPlugin[] { evt, daily, streak });

            var claimed = mgr.InvokeShowPopup(Placements.MapScreen, Ctx(Placements.MapScreen));

            Assert.True(claimed);
            Assert.Equal(new[] { "popup" }, streak.Calls);
            Assert.Equal(new[] { "popup" }, daily.Calls);
            Assert.Empty(evt.Calls);
        }

        [Fact]
        public void ShowPopup_NoClaimer_ReturnsFalse()
        {
            var flags = new FakeFeatureFlags().Enable("a");
            var a = new RecordingPlugin("a", false, (Placements.Store, 100));
            var mgr = new PluginManager(flags, new ListLog(), new[] { a });

            Assert.False(mgr.InvokeShowPopup(Placements.Store, Ctx(Placements.Store)));
            Assert.False(mgr.InvokeShowPopup(Placements.Inbox, Ctx(Placements.Inbox)));
        }

        [Fact]
        public void PluginException_IsLogged_AndOthersStillRun()
        {
            var flags = new FakeFeatureFlags().Enable("thrower", "a");
            var log = new ListLog();
            var a = new RecordingPlugin("a", false, (Placements.MapScreen, 20));
            var mgr = new PluginManager(flags, log, new IPlugin[] { new ThrowingPlugin(), a });

            mgr.Invoke(Placements.MapScreen, Ctx(Placements.MapScreen), PluginHook.Init);

            Assert.Single(log.Errors);
            Assert.Contains("thrower", log.Errors[0].Message);
            Assert.Equal(new[] { "init" }, a.Calls);
        }

        [Fact]
        public void ButtonPressed_PassesButtonName()
        {
            var flags = new FakeFeatureFlags().Enable("a");
            var a = new RecordingPlugin("a", false, (Placements.DailyBonus, 100));
            var mgr = new PluginManager(flags, new ListLog(), new[] { a });

            mgr.Invoke(Placements.DailyBonus, Ctx(Placements.DailyBonus), PluginHook.ButtonPressed, "claim");

            Assert.Equal(new[] { "button:claim" }, a.Calls);
        }

        [Fact]
        public void PriorityOverride_FromFlags_Reorders()
        {
            var flags = new FakeFeatureFlags().Enable("a", "b");
            flags.Values["plugin.a.priority.MAP_SCREEN"] = 5;
            var a = new RecordingPlugin("a", false, (Placements.MapScreen, 300));
            var b = new RecordingPlugin("b", false, (Placements.MapScreen, 100));
            var mgr = new PluginManager(flags, new ListLog(), new IPlugin[] { a, b });

            Assert.Equal(new[] { "a", "b" }, mgr.GetPlugins(Placements.MapScreen).Select(p => p.PluginId));
        }

        [Fact]
        public void FlagTurnedOffAfterInit_PluginIsSkippedOnInvoke()
        {
            var flags = new FakeFeatureFlags().Enable("a");
            var a = new RecordingPlugin("a", false, (Placements.MapScreen, 100));
            var mgr = new PluginManager(flags, new ListLog(), new[] { a });

            flags.Values["a"] = false;
            mgr.Invoke(Placements.MapScreen, Ctx(Placements.MapScreen), PluginHook.Init);

            Assert.Empty(a.Calls);
        }

        [Fact]
        public void SamePlugin_DifferentPriorityPerPlacement()
        {
            var flags = new FakeFeatureFlags().Enable("a", "b");
            var a = new RecordingPlugin("a", false, (Placements.MapScreen, 100), (Placements.Store, 900));
            var b = new RecordingPlugin("b", false, (Placements.MapScreen, 200), (Placements.Store, 100));
            var mgr = new PluginManager(flags, new ListLog(), new IPlugin[] { a, b });

            Assert.Equal(new[] { "a", "b" }, mgr.GetPlugins(Placements.MapScreen).Select(p => p.PluginId));
            Assert.Equal(new[] { "b", "a" }, mgr.GetPlugins(Placements.Store).Select(p => p.PluginId));
        }
    }
}
