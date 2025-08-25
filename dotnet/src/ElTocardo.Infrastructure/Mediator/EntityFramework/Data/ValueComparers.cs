using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ElTocardo.Infrastructure.Mediator.EntityFramework.Data;

public static class ValueComparers
{
    public static ValueComparer<IDictionary<string, string?>> DictionaryStringNullableStringComparer { get; } =
        new( (d1, d2) =>DictionaryStringNullableStringEqualsExpression(d1,d2),d=>DictionaryStringNullableStringHashCodeExpression(d),c=>DictionaryStringNullableStringSnapshotExpression(c));


    public static ValueComparer<IList<string>> ListStringComparer { get; } =
        new((d1, d2) => ListEqualsExpression(d1, d2), d=> ListHashCodeExpression(d), c => ListSnapshotExpression(c));

    private static Dictionary<string,string?> DictionaryStringNullableStringSnapshotExpression(IDictionary<string,string?> d)
    {
        return d.ToDictionary(
            entry => entry.Key,
            entry => entry.Value
        );
    }

    private static int DictionaryStringNullableStringHashCodeExpression(IDictionary<string,string?> d)
    {
        return d.Aggregate(0, (a, v) =>
            HashCode.Combine(a, v.Key.GetHashCode(), v.Value?.GetHashCode() ?? 0));
    }

    private static bool DictionaryStringNullableStringEqualsExpression(IDictionary<string,string?>? d1, IDictionary<string,string?>? d2)
    {
        return ReferenceEquals(d1, d2) ||
               (d1 != null && d2 != null &&
                d1.Count == d2.Count &&
                d1.All(kv => d2.TryGetValue(kv.Key, out var v) && kv.Value == v));
    }

    private static IList<T> ListSnapshotExpression<T>(IList<T> d)
    {
        return [.. d];
    }


    private static int ListHashCodeExpression<T>(IList<T> c)
    {
        return c.Aggregate(0, (a, v) => HashCode.Combine(a, v != null ? v.GetHashCode() : 0));
    }

    private static bool ListEqualsExpression<T>(IList<T>? d1, IList<T>? d2)
    {
        return ReferenceEquals(d1, d2) ||
               (d1 != null && d2 != null && d1.SequenceEqual(d2));
    }
}
