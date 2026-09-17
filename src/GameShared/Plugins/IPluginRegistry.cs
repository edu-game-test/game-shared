namespace MetaFramework.Plugins
{
    public interface IPluginRegistry
    {
        /// <summary>Register interest in a placement. Lower priority value = called first.</summary>
        void RegisterPlacement(string placement, int priority);
    }
}
