namespace MetaFramework.Common
{
    /// <summary>Canonical platform ids (feature-flag-system.md). segmentation-system.md's "pc" is normalised to "steam".</summary>
    public static class Platforms
    {
        public const string Android = "android";
        public const string Ios = "ios";
        public const string Steam = "steam";
        public const string Web = "web";
        public static readonly string[] All = { Android, Ios, Steam, Web };
        public static bool IsValid(string? p) => p == Android || p == Ios || p == Steam || p == Web;
    }
}
