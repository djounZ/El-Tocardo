using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;

public sealed record ReasoningOptionsDto(ReasoningEffortEnumDto? Effort, ReasoningOutputEnumDto? Output );
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReasoningOutputEnumDto
{
    [JsonStringEnumMemberName("none")] None = 1,

    [JsonStringEnumMemberName("summary")] Summary = 2,
    [JsonStringEnumMemberName("full")] Full = 3
}
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReasoningEffortEnumDto
{
    [JsonStringEnumMemberName("none")] None = 1,

    [JsonStringEnumMemberName("low")] Low = 2,
    [JsonStringEnumMemberName("medium")] Medium = 3,
    [JsonStringEnumMemberName("high")] High = 4,
    [JsonStringEnumMemberName("extra_high")] ExtraHigh = 5
}
