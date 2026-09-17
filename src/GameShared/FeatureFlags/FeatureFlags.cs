namespace MetaFramework.FeatureFlags
{
    /// <summary>
    /// Canonical flag key constants. Keep alphabetical within each group. The default documented next to each
    /// constant MUST match the defaultValue passed at every call site (feature-flag-system.md "Convention").
    /// Plugin master switches are the plugin's PluginId and are not listed here.
    /// </summary>
    public static class FeatureFlags
    {
        // Daily Bonus
        public const string DailyBonusEnabled  = "daily_bonus_enabled";
        public const string DailyBonusVariant  = "daily_bonus_variant";
        public const string DailyBonusConfig   = "daily_bonus_config";

        // Events (Phase 2 — flag reserved on Day 1)
        public const string EventsEnabled      = "events_enabled";
        public const string MaxEventSlots      = "max_event_slots";

        // Shop
        public const string BoosterShopEnabled = "booster_shop_enabled";
        public const string BoosterDiscount    = "booster_discount";

        // Social (Phase 2 — reserved)
        public const string TeamsEnabled       = "teams_enabled";
        public const string LeaderboardEnabled = "leaderboard_enabled";
    }
}
