using System.Text.Json.Serialization;
using ElTocardo.Application.Dtos.AI.ChatCompletion.Request;

namespace ElTocardo.Application.Dtos.AI.ChatCompletion.Response;

public sealed record ChatResponseDto(
    [property: JsonPropertyName("messages")]
    IList<ChatMessageDto> Messages,
    [property: JsonPropertyName("response_id")]
    string? ResponseId,
    [property: JsonPropertyName("conversation_id")]
    string? ConversationId,
    [property: JsonPropertyName("model_id")]
    string? ModelId,
    [property: JsonPropertyName("created_at")]
    DateTimeOffset? CreatedAt,
    [property: JsonPropertyName("finish_reason")]
    ChatFinishReasonDto? FinishReason
);
