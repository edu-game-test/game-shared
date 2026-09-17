using System.Collections.Generic;
using MetaFramework.Economy;

namespace MetaFramework.Contract
{
    /// <summary>Declares every resource (currency, energy, booster, cosmetic token) the game uses.</summary>
    public interface IGameEconomy
    {
        IReadOnlyList<ResourceDefinition> Resources { get; }
    }
}
