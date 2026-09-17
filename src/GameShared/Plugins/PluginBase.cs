namespace MetaFramework.Plugins
{
    /// <summary>No-op defaults so plugins only override the hooks they use (answers plugin-system.md open question 4 with option b).</summary>
    public abstract class PluginBase : IPlugin
    {
        public abstract string PluginId { get; }
        public abstract void Register(IPluginRegistry registry);
        public virtual bool OnShowPopup(PlacementContext ctx) => false;
        public virtual void OnInit(PlacementContext ctx) { }
        public virtual void OnUpdate(PlacementContext ctx) { }
        public virtual void OnClose(PlacementContext ctx) { }
        public virtual void OnButtonPressed(PlacementContext ctx, string buttonName) { }
    }
}
