namespace MetaFramework.Plugins
{
    /// <summary>Lets a plugin decorate the current screen. Unity adapter maps widget to GameObject and element to Component.</summary>
    public interface IScreenContext
    {
        void AddWidget(string slotName, object widget);
        void RemoveWidget(string slotName);
        T? GetElement<T>(string elementName) where T : class;
    }
}
