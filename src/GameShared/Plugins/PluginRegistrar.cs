using System.Collections.Generic;

namespace MetaFramework.Plugins
{
    internal sealed class PluginRegistrar : IPluginRegistry
    {
        public List<(string Placement, int Priority)> Registrations { get; } = new();
        public void RegisterPlacement(string placement, int priority) => Registrations.Add((placement, priority));
    }
}
