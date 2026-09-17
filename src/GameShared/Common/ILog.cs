using System;

namespace MetaFramework.Common
{
    /// <summary>Platform-neutral logger. Unity adapter maps to Debug.Log*, server adapter to ILogger.</summary>
    public interface ILog
    {
        void Info(string message);
        void Warn(string message);
        void Error(string message, Exception? exception = null);
    }
}
