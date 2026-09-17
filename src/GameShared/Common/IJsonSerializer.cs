namespace MetaFramework.Common
{
    /// <summary>JSON abstraction so shared code never depends on System.Text.Json (server) or Newtonsoft (Unity).</summary>
    public interface IJsonSerializer
    {
        string Serialize(object value);
        T Deserialize<T>(string json);
    }
}
