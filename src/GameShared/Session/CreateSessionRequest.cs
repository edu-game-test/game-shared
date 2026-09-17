namespace MetaFramework.Session
{
    /// <summary>POST /api/v1/auth/session. X-Game-Id header carries the game id.</summary>
    public sealed class CreateSessionRequest
    {
        /// <summary>Firebase ID token (anonymous for Guest, provider token otherwise).</summary>
        public string FirebaseIdToken { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public string ClientVersion { get; set; } = "0.0.0";
        public string Locale { get; set; } = "en";
        public string DeviceId { get; set; } = string.Empty;
    }
}
