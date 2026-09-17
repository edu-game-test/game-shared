namespace MetaFramework.Tracking
{
    public interface ITrackingService
    {
        /// <summary>Enqueue an event. Never blocks; batching and delivery are the implementation's concern.</summary>
        void Send(TrackingEvent evt);
        /// <summary>Set a sticky player property (e.g. A/B flag assignments) sent with every batch.</summary>
        void SetPlayerProperty(string key, object value);
    }
}
