using System;
using System.Collections.Generic;
using System.Linq;
using MetaFramework.Common;
using MetaFramework.FeatureFlags;

namespace MetaFramework.Plugins
{
    /// <summary>
    /// Discovers plugins, gates them on feature flags, keeps a priority-sorted registry per placement,
    /// and dispatches lifecycle hooks (plugin-system.md "PluginManager"). Pure C#; the Unity bootstrap wraps it.
    /// </summary>
    public sealed class PluginManager
    {
        private static readonly IReadOnlyList<IPlugin> Empty = new List<IPlugin>();

        private readonly Dictionary<string, List<(int Priority, IPlugin Plugin)>> _registry = new();
        private readonly IFeatureFlagService _flags;
        private readonly ILog _log;

        public PluginManager(IFeatureFlagService flags, ILog log, IEnumerable<IPlugin> plugins)
        {
            _flags = flags;
            _log = log;
            Initialize(plugins);
        }

        private void Initialize(IEnumerable<IPlugin> plugins)
        {
            foreach (var plugin in plugins)
            {
                if (!_flags.IsEnabled(plugin.PluginId))
                {
                    _log.Info($"[PluginManager] Plugin '{plugin.PluginId}' disabled by feature flag. Skipping.");
                    continue;
                }

                var registrar = new PluginRegistrar();
                try
                {
                    plugin.Register(registrar);
                }
                catch (Exception ex)
                {
                    _log.Error($"[PluginManager] {plugin.PluginId}.Register threw", ex);
                    continue;
                }

                foreach (var (placement, codePriority) in registrar.Registrations)
                {
                    var priority = _flags.GetInt($"plugin.{plugin.PluginId}.priority.{placement}", codePriority);
                    if (!_registry.TryGetValue(placement, out var list))
                        _registry[placement] = list = new List<(int, IPlugin)>();
                    list.Add((priority, plugin));
                }
            }

            foreach (var list in _registry.Values)
                StableSortByPriority(list);
        }

        /// <summary>Stable sort: equal priorities keep registration order (plugin-system.md "ShowPopup Winner Resolution").</summary>
        private static void StableSortByPriority(List<(int Priority, IPlugin Plugin)> list)
        {
            var sorted = list.Select((entry, index) => (entry, index))
                             .OrderBy(x => x.entry.Priority)
                             .ThenBy(x => x.index)
                             .Select(x => x.entry)
                             .ToList();
            list.Clear();
            list.AddRange(sorted);
        }

        public IReadOnlyList<IPlugin> GetPlugins(string placement) =>
            _registry.TryGetValue(placement, out var list) ? list.Select(e => e.Plugin).ToList() : Empty;

        /// <summary>Returns true if any plugin claimed the popup slot. Stops at the first claimer.</summary>
        public bool InvokeShowPopup(string placement, PlacementContext ctx)
        {
            if (!_registry.TryGetValue(placement, out var plugins))
                return false;

            foreach (var (_, plugin) in plugins)
            {
                if (!_flags.IsEnabled(plugin.PluginId)) continue;
                try
                {
                    if (plugin.OnShowPopup(ctx))
                        return true;
                }
                catch (Exception ex)
                {
                    LogPluginException(plugin, "OnShowPopup", placement, ex);
                }
            }
            return false;
        }

        /// <summary>Invoke a non-popup hook. Every registered (and still enabled) plugin is called in priority order.</summary>
        public void Invoke(string placement, PlacementContext ctx, PluginHook hook, string? buttonName = null)
        {
            if (!_registry.TryGetValue(placement, out var plugins))
                return;

            foreach (var (_, plugin) in plugins)
            {
                if (!_flags.IsEnabled(plugin.PluginId)) continue;
                try
                {
                    switch (hook)
                    {
                        case PluginHook.Init:          plugin.OnInit(ctx); break;
                        case PluginHook.Update:        plugin.OnUpdate(ctx); break;
                        case PluginHook.Close:         plugin.OnClose(ctx); break;
                        case PluginHook.ButtonPressed: plugin.OnButtonPressed(ctx, buttonName ?? string.Empty); break;
                    }
                }
                catch (Exception ex)
                {
                    LogPluginException(plugin, hook.ToString(), placement, ex);
                }
            }
        }

        private void LogPluginException(IPlugin plugin, string hook, string placement, Exception ex) =>
            _log.Error($"[PluginManager] {plugin.PluginId}.{hook}@{placement} threw", ex);
    }
}
