namespace MetaFramework.Plugins
{
    /// <summary>All meta-feature plugins implement this. PluginId must equal the plugin's feature-flag key.</summary>
    public interface IPlugin
    {
        /// <summary>snake_case, e.g. "daily_bonus". Doubles as the master feature-flag key.</summary>
        string PluginId { get; }
        void Register(IPluginRegistry registry);
        /// <summary>Return true to claim the popup slot; only the first claimer (by priority) is awarded it.</summary>
        bool OnShowPopup(PlacementContext ctx);
        void OnInit(PlacementContext ctx);
        void OnUpdate(PlacementContext ctx);
        void OnClose(PlacementContext ctx);
        void OnButtonPressed(PlacementContext ctx, string buttonName);
    }
}
