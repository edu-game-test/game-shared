using System.Collections.Generic;

namespace MetaFramework.Screens
{
    /// <summary>Arbitrary key-value payload passed to a screen on open.</summary>
    public sealed class ScreenContext
    {
        public Dictionary<string, object> Data { get; } = new();

        public T Get<T>(string key) => (T)Data[key];
        public bool TryGet<T>(string key, out T value)
        {
            if (Data.TryGetValue(key, out var obj) && obj is T typed) { value = typed; return true; }
            value = default!;
            return false;
        }
        public ScreenContext Set<T>(string key, T value) { Data[key] = value!; return this; }
    }
}
