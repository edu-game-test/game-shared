namespace MetaFramework.FeatureFlags
{
    /// <summary>Minimal "major.minor.patch" comparison. Non-numeric parts are treated as 0.</summary>
    public static class SemVer
    {
        public static int Compare(string a, string b)
        {
            var pa = Parse(a); var pb = Parse(b);
            for (int i = 0; i < 3; i++)
            {
                if (pa[i] != pb[i]) return pa[i].CompareTo(pb[i]);
            }
            return 0;
        }

        private static int[] Parse(string v)
        {
            var result = new int[3];
            var parts = (v ?? string.Empty).Split('.');
            for (int i = 0; i < 3 && i < parts.Length; i++)
            {
                var numeric = parts[i].Split('-')[0];
                result[i] = int.TryParse(numeric, out var n) ? n : 0;
            }
            return result;
        }
    }
}
