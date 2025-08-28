using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.AI.ChatCompletion.Request;

public sealed record ChatOptionsDto(
    [property: JsonPropertyName("conversation_id")]
    string? ConversationId,
    [property: JsonPropertyName("instructions")]
    string? Instructions,
    [property: JsonPropertyName("temperature")]
    float? Temperature,
    [property: JsonPropertyName("max_output_tokens")]
    int? MaxOutputTokens,
    [property: JsonPropertyName("top_p")] float? TopP,
    [property: JsonPropertyName("top_k")] int? TopK,
    [property: JsonPropertyName("frequency_penalty")]
    float? FrequencyPenalty,
    [property: JsonPropertyName("presence_penalty")]
    float? PresencePenalty,
    [property: JsonPropertyName("seed")] long? Seed,
    [property: JsonPropertyName("response_format")]
    ChatResponseFormatDto? ResponseFormat,
    [property: JsonPropertyName("model_id")]
    string? ModelId,
    [property: JsonPropertyName("stop_sequences")]
    IList<string>? StopSequences,
    [property: JsonPropertyName("allow_multiple_tool_calls")]
    bool? AllowMultipleToolCalls,
    [property: JsonPropertyName("tool_mode")]
    ChatToolModeDto? ToolMode,
    [property: JsonPropertyName("tools")] IDictionary<string, IList<AiToolDto>>? Tools
);
