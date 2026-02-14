using System.Text.Json.Serialization;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;

namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;

public sealed record ChatResponseUpdateDto(
    [property: JsonPropertyName("author_name")]
    string? AuthorName,
    [property: JsonPropertyName("role")] ChatRoleEnumDto? Role,
    [property: JsonPropertyName("contents")]
    IList<AiContentDto> Contents,
    [property: JsonPropertyName("response_id")]
    string? ResponseId,
    [property: JsonPropertyName("message_id")]
    string? MessageId,
    [property: JsonPropertyName("conversation_id")]
    string? ConversationId,
    [property: JsonPropertyName("created_at")]
    DateTimeOffset? CreatedAt,
    [property: JsonPropertyName("finish_reason")]
    ChatFinishReasonDto? FinishReason,
    [property: JsonPropertyName("model_id")]
    string? ModelId
);
