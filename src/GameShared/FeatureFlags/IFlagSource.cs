using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MetaFramework.FeatureFlags
{
    /// <summary>Fetches the server-resolved flag map (GET /api/v1/flags). Throws on network failure.</summary>
    public interface IFlagSource
    {
        Task<IReadOnlyDictionary<string, FlagValue>> FetchAsync(CancellationToken ct);
    }
}
