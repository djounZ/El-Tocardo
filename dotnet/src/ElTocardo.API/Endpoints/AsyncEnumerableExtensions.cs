using System.Reflection;
using System.Text.Json.Serialization;

namespace ElTocardo.API.Endpoints;

public static class AsyncEnumerableExtensions
{

    private const string StreamingContentType = "application/json-stream";
    public static IResult ToResult<T>(this IAsyncEnumerable<T> responseStream, CancellationToken cancellationToken)
    {
        return Results.Stream(
            async (stream) =>
            {
                await foreach (var update in responseStream.WithCancellation(cancellationToken))
                {
                    var data = System.Text.Json.JsonSerializer.Serialize(update);
                    var bytes = System.Text.Encoding.UTF8.GetBytes(data +  Environment.NewLine);
                    await stream.WriteAsync(bytes, cancellationToken);
                    await stream.FlushAsync(cancellationToken);
                }
            },
            contentType: StreamingContentType
        );
    }



    public static TEnum Parse<TEnum>(this string value) where TEnum : struct, Enum
    {
        foreach (var field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var attr = field.GetCustomAttribute<JsonStringEnumMemberNameAttribute>();
            if (attr != null && string.Equals(attr.Name, value, StringComparison.OrdinalIgnoreCase))
            {
                return (TEnum)field.GetValue(null)!;
            }
        }
        throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid string value for enum conversion.");
    }
}
