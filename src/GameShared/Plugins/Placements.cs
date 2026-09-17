namespace MetaFramework.Plugins
{
    /// <summary>Named moments in the game lifecycle where plugins can hook in (plugin-system.md "Placements Reference").</summary>
    public static class Placements
    {
        public const string MapScreen     = "MAP_SCREEN";
        public const string PreLevel      = "PRE_LEVEL";
        public const string PostLevelWin  = "POST_LEVEL_WIN";
        public const string PostLevelLose = "POST_LEVEL_LOSE";
        public const string DailyBonus    = "DAILY_BONUS";
        public const string Store         = "STORE";
        public const string Settings      = "SETTINGS";
        public const string Inbox         = "INBOX";
        public const string Profile       = "PROFILE";
        public const string EventsList    = "EVENTS_LIST";
    }
}
