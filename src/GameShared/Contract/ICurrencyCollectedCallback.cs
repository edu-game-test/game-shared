namespace MetaFramework.Contract
{
    /// <summary>Called by the game during gameplay whenever an in-level currency is collected.</summary>
    public interface ICurrencyCollectedCallback
    {
        void OnCurrencyCollected(string currencyId, int amount, WorldPosition worldPosition);
    }
}
