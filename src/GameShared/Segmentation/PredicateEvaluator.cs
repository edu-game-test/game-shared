using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MetaFramework.Segmentation
{
    /// <summary>Applies one predicate operator to a resolved field value. Numeric comparisons go through double.</summary>
    public static class PredicateEvaluator
    {
        public static bool MatchesAll(SegmentDefinition def, Func<string, object?> resolveField)
        {
            if (def.Predicates.Count == 0) return false;
            var results = def.Predicates.Select(p => Matches(p, resolveField(p.Field)));
            return def.PredicateLogic == PredicateLogic.And ? results.All(r => r) : results.Any(r => r);
        }

        public static bool Matches(Predicate p, object? fieldValue)
        {
            switch (p.Op)
            {
                case "exists": return fieldValue != null;
                case "eq":     return fieldValue != null && ValuesEqual(fieldValue, p.Value);
                case "neq":    return fieldValue == null || !ValuesEqual(fieldValue, p.Value);
                case "gt":     return TryNumbers(fieldValue, p.Value, out var a1, out var b1) && a1 > b1;
                case "gte":    return TryNumbers(fieldValue, p.Value, out var a2, out var b2) && a2 >= b2;
                case "lt":     return TryNumbers(fieldValue, p.Value, out var a3, out var b3) && a3 < b3;
                case "lte":    return TryNumbers(fieldValue, p.Value, out var a4, out var b4) && a4 <= b4;
                case "in":     return fieldValue != null && AsList(p.Value).Any(v => ValuesEqual(fieldValue, v));
                case "not_in": return fieldValue == null || !AsList(p.Value).Any(v => ValuesEqual(fieldValue, v));
                case "between":
                {
                    var range = AsList(p.Value).ToList();
                    if (range.Count != 2 || !TryNumber(fieldValue, out var x)
                        || !TryNumber(range[0], out var lo) || !TryNumber(range[1], out var hi)) return false;
                    return x >= lo && x <= hi;
                }
                default: return false;
            }
        }

        private static IEnumerable<object?> AsList(object? value)
        {
            if (value is string s) return new object?[] { s };
            if (value == null) return Enumerable.Empty<object?>();
            if (value is IEnumerable e) return e.Cast<object?>();
            return new[] { value };
        }

        private static bool ValuesEqual(object? a, object? b)
        {
            if (a == null || b == null) return a == null && b == null;
            if (TryNumber(a, out var na) && TryNumber(b, out var nb)) return Math.Abs(na - nb) < 1e-9;
            if (a is bool ba && b is bool bb) return ba == bb;
            return string.Equals(a.ToString(), b.ToString(), StringComparison.Ordinal);
        }

        private static bool TryNumbers(object? a, object? b, out double na, out double nb)
        {
            nb = 0;
            return TryNumber(a, out na) && TryNumber(b, out nb);
        }

        private static bool TryNumber(object? v, out double d)
        {
            switch (v)
            {
                case null: d = 0; return false;
                case bool: d = 0; return false;
                case string sv: return double.TryParse(sv, NumberStyles.Float, CultureInfo.InvariantCulture, out d);
                case IConvertible c:
                    try { d = c.ToDouble(CultureInfo.InvariantCulture); return true; }
                    catch { d = 0; return false; }
                default: d = 0; return false;
            }
        }
    }
}
