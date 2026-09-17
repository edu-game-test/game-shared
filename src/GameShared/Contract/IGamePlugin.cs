namespace MetaFramework.Contract
{
    /// <summary>Root registration interface. One implementation per game.</summary>
    public interface IGamePlugin
    {
        /// <summary>Unique game id, e.g. "match3". Used in analytics and server routing.</summary>
        string GameId { get; }
        string GameDisplayName { get; }
        /// <summary>Called once after all meta systems have initialized.</summary>
        void OnMetaReady(IMetaFramework meta);
        /// <summary>Called when the framework needs the game to return to the map (force-close, session restore).</summary>
        void OnReturnToMeta();
    }
}
