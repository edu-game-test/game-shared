using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MetaFramework.Common;

namespace MetaFramework.FeatureFlags
{
    /// <summary>
    /// Fetch-at-start, cache-locally, read-synchronously flag service (feature-flag-system.md "Fetch and cache lifecycle").
    /// Evaluation on the client is only: local override → resolved map (server or cache) → hardcoded default.
    /// The map reference is swapped atomically on refresh; reads are lock-free.
    /// </summary>
    public sealed class FeatureFlagService : IFeatureFlagService
    {
        private static readonly IReadOnlyDictionary<string, FlagValue> EmptyMap = new Dictionary<string, FlagValue>();
        private static readonly TimeSpan DefaultInitTimeout = TimeSpan.FromSeconds(5);

        private readonly IFlagSource _source;
        private readonly IFlagCache? _cache;
        private readonly IJsonSerializer _json;
        private readonly ILog _log;
        private readonly bool _allowLocalOverrides;
        private readonly TimeSpan _initTimeout;
        private readonly Dictionary<string, FlagValue> _localOverrides = new();
        private readonly object _overrideLock = new();

        private IReadOnlyDictionary<string, FlagValue> _resolved = EmptyMap;

        public FeatureFlagService(IFlagSource source, IFlagCache? cache, IJsonSerializer json, ILog log,
                                  bool allowLocalOverrides, TimeSpan? initTimeout = null)
        {
            _source = source;
            _cache = cache;
            _json = json;
            _log = log;
            _allowLocalOverrides = allowLocalOverrides;
            _initTimeout = initTimeout ?? DefaultInitTimeout;
        }

        public bool IsInitialized { get; private set; }
        public event Action? OnFlagsRefreshed;

        /// <summary>Loads the cache, then fetches. Returns after the fetch or after the timeout — whichever is first.</summary>
        public async Task InitializeAsync(CancellationToken ct)
        {
            var cached = SafeLoadCache();
            if (cached != null)
                Interlocked.Exchange(ref _resolved, cached);

            var fetch = FetchAndApplyAsync(ct);
            var completed = await Task.WhenAny(fetch, Task.Delay(_initTimeout, ct)).ConfigureAwait(false);
            if (completed != fetch)
                _log.Warn($"[FeatureFlags] Fetch exceeded {_initTimeout.TotalSeconds:0.#}s; continuing with {(cached != null ? "cached" : "default")} values.");
        }

        public Task RefreshAsync(CancellationToken ct) => FetchAndApplyAsync(ct);

        private async Task FetchAndApplyAsync(CancellationToken ct)
        {
            try
            {
                var fresh = await _source.FetchAsync(ct).ConfigureAwait(false);
                Interlocked.Exchange(ref _resolved, fresh);
                IsInitialized = true;
                SafeSaveCache(fresh);
                OnFlagsRefreshed?.Invoke();
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                _log.Error("[FeatureFlags] Fetch failed; using cached/default values.", ex);
            }
        }

        private IReadOnlyDictionary<string, FlagValue>? SafeLoadCache()
        {
            try { return _cache?.Load(); }
            catch (Exception ex) { _log.Warn("[FeatureFlags] Cache load failed: " + ex.Message); return null; }
        }

        private void SafeSaveCache(IReadOnlyDictionary<string, FlagValue> flags)
        {
            try { _cache?.Save(flags); }
            catch (Exception ex) { _log.Warn("[FeatureFlags] Cache save failed: " + ex.Message); }
        }

        private bool TryResolve(string flagKey, out FlagValue value)
        {
            lock (_overrideLock)
            {
                if (_localOverrides.TryGetValue(flagKey, out value))
                    return true;
            }
            return _resolved.TryGetValue(flagKey, out value);
        }

        public bool IsEnabled(string flagKey) => TryResolve(flagKey, out var v) && v.AsBool();
        public string GetString(string flagKey, string defaultValue = "") => TryResolve(flagKey, out var v) ? v.AsString(defaultValue) : defaultValue;
        public int GetInt(string flagKey, int defaultValue = 0) => TryResolve(flagKey, out var v) ? v.AsInt(defaultValue) : defaultValue;
        public float GetFloat(string flagKey, float defaultValue = 0f) => TryResolve(flagKey, out var v) ? v.AsFloat(defaultValue) : defaultValue;

        public T GetJson<T>(string flagKey, T defaultValue = default!)
        {
            if (!TryResolve(flagKey, out var v) || v.Kind != FlagKind.Json || v.StringValue == null)
                return defaultValue;
            try { return _json.Deserialize<T>(v.StringValue); }
            catch (Exception ex)
            {
                _log.Error($"[FeatureFlags] Flag '{flagKey}' is not valid JSON for {typeof(T).Name}; using default.", ex);
                return defaultValue;
            }
        }

        public void SetLocalOverride(string flagKey, object value)
        {
            EnsureOverridesAllowed();
            lock (_overrideLock) _localOverrides[flagKey] = FlagValue.FromObject(value, _json);
        }

        public void ClearLocalOverride(string flagKey)
        {
            EnsureOverridesAllowed();
            lock (_overrideLock) _localOverrides.Remove(flagKey);
        }

        public void ClearAllLocalOverrides()
        {
            EnsureOverridesAllowed();
            lock (_overrideLock) _localOverrides.Clear();
        }

        private void EnsureOverridesAllowed()
        {
            if (!_allowLocalOverrides)
                throw new NotSupportedException("Local feature-flag overrides are disabled in production builds.");
        }
    }
}
