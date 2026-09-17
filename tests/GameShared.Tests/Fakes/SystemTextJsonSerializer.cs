using System.Text.Json;
using System.Text.Json.Serialization;
using MetaFramework.Common;

namespace GameShared.Tests.Fakes
{
    public sealed class SystemTextJsonSerializer : IJsonSerializer
    {
        private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        };

        public string Serialize(object value) => JsonSerializer.Serialize(value, Options);
        public T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, Options)!;
    }
}
