namespace MetaFramework.Segmentation
{
    /// <summary>Predicate field names (segmentation-system.md "Predicate Fields"). ab_bucket_{id} is virtual.</summary>
    public static class SegmentFields
    {
        public const string LevelNumber = "level_number";
        public const string StarsTotal = "stars_total";
        public const string DaysSinceInstall = "days_since_install";
        public const string SessionCount7d = "session_count_7d";
        public const string LifetimeSpendUsd = "lifetime_spend_usd";
        public const string Platform = "platform";
        public const string CountryCode = "country_code";
        public const string HasPurchased = "has_purchased";
        public const string LastSessionDaysAgo = "last_session_days_ago";
        public const string AbBucketPrefix = "ab_bucket_";
        public static readonly string[] Known =
        {
            LevelNumber, StarsTotal, DaysSinceInstall, SessionCount7d, LifetimeSpendUsd,
            Platform, CountryCode, HasPurchased, LastSessionDaysAgo,
        };
    }
}
