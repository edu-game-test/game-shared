using System;
using System.Globalization;
using MetaFramework.Common;

namespace MetaFramework.FeatureFlags
{
    public enum FlagKind { Bool, String, Number, Json }

    /// <summary>A resolved flag value. Server sends native JSON types; adapters map them into this struct.</summary>
    public readonly struct FlagValue
    {
        private FlagValue(FlagKind kind, bool b, string? s, double n)
        {
            Kind = kind; BoolValue = b; StringValue = s; NumberValue = n;
        }

        public FlagKind Kind { get; }
        public bool BoolValue { get; }
        /// <summary>For String: the value. For Json: the raw JSON text.</summary>
        public string? StringValue { get; }
        public double NumberValue { get; }

        public static FlagValue Bool(bool value) => new(FlagKind.Bool, value, null, 0);
        public static FlagValue String(string value) => new(FlagKind.String, false, value, 0);
        public static FlagValue Number(double value) => new(FlagKind.Number, false, null, value);
        public static FlagValue Json(string rawJson) => new(FlagKind.Json, false, rawJson, 0);

        /// <summary>Maps a CLR value (from SetLocalOverride or a deserialized dictionary) to a FlagValue.</summary>
        public static FlagValue FromObject(object value, IJsonSerializer json)
        {
            switch (value)
            {
                case bool b: return Bool(b);
                case string s: return String(s);
                case int i: return Number(i);
                case long l: return Number(l);
                case float f: return Number(f);
                case double d: return Number(d);
                case decimal m: return Number((double)m);
                default: return Json(json.Serialize(value));
            }
        }

        public bool AsBool()
        {
            switch (Kind)
            {
                case FlagKind.Bool: return BoolValue;
                case FlagKind.String: return bool.TryParse(StringValue, out var b) && b;
                case FlagKind.Number: return NumberValue != 0;
                default: return false;
            }
        }

        public string AsString(string defaultValue)
        {
            switch (Kind)
            {
                case FlagKind.String: return StringValue ?? defaultValue;
                case FlagKind.Bool: return BoolValue ? "true" : "false";
                case FlagKind.Number: return NumberValue.ToString(CultureInfo.InvariantCulture);
                default: return StringValue ?? defaultValue;
            }
        }

        public int AsInt(int defaultValue)
        {
            switch (Kind)
            {
                case FlagKind.Number: return (int)Math.Round(NumberValue);
                case FlagKind.String: return int.TryParse(StringValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i) ? i : defaultValue;
                default: return defaultValue;
            }
        }

        public float AsFloat(float defaultValue)
        {
            switch (Kind)
            {
                case FlagKind.Number: return (float)NumberValue;
                case FlagKind.String: return float.TryParse(StringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var f) ? f : defaultValue;
                default: return defaultValue;
            }
        }
    }
}
