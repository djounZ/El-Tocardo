using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.AI.ChatCompletion.Response;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ChatFinishReasonDto
{
    [JsonStringEnumMemberName("stop")] Stop = 1,
    [JsonStringEnumMemberName("length")] Length = 2,

    [JsonStringEnumMemberName("tool_calls")]
    ToolCalls = 3,

    [JsonStringEnumMemberName("content_filter")]
    ContentFilter = 4
}
