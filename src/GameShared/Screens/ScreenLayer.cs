namespace MetaFramework.Screens
{
    /// <summary>Draw order and input behaviour. UI Toolkit PanelSettings.sortingOrder = (int)Layer * 100 (screen-system.md "Input Blocking", Decision D6).</summary>
    public enum ScreenLayer { Background = 0, Fullscreen = 1, Overlay = 2, Popup = 3, HUD = 4, System = 5 }
}
