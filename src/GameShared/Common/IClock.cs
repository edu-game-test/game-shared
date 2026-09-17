using System;

namespace MetaFramework.Common
{
    /// <summary>Source of the current UTC time. Inject this instead of calling DateTime.UtcNow so tests can control time.</summary>
    public interface IClock
    {
        DateTime UtcNow { get; }
    }
}
