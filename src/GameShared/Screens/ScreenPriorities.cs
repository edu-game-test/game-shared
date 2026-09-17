namespace MetaFramework.Screens
{
    /// <summary>Queue priorities. Lower = more urgent (screen-system.md "Priority Table").</summary>
    public static class ScreenPriorities
    {
        public const int Tutorial      = 5;
        public const int SystemAlert   = 10;
        public const int LevelResult   = 20;
        public const int DailyLogin    = 30;
        public const int EventPopup    = 40;
        public const int InboxBadge    = 50;
        public const int StreakOffer   = 60;
        public const int Promo         = 100;
        public const int OnboardingTip = 200;
    }
}
