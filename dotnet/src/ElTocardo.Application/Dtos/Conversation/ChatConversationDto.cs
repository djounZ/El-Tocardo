using System.Text.Json.Serialization;
using ElTocardo.Application.Dtos.AI.ChatCompletion.Request;
using ElTocardo.Application.Dtos.AI.ChatCompletion.Response;
using ElTocardo.Application.Dtos.AI.Contents;
using ElTocardo.Application.Dtos.Configuration;

namespace ElTocardo.Application.Dtos.Conversation;


public sealed record StartConversationRequestDto(
    [property: JsonPropertyName("title")]
    string Title,
    [property: JsonPropertyName("description")]
    string? Description,
    [property: JsonPropertyName("input_message")]
    ChatMessageDto InputMessage,
    [property: JsonPropertyName("provider")]
    AiProviderEnumDto? Provider = null,
    [property: JsonPropertyName("options")]
    ChatOptionsDto? Options = null,
    [property: JsonPropertyName("is_public")]
    bool? IsPublic = null);


public sealed record ContinueConversationDto(
    [property: JsonPropertyName("conversation_id")]
    string ConversationId,
    [property: JsonPropertyName("input_message")]
    ChatMessageDto InputMessage,
    [property: JsonPropertyName("provider")]
    AiProviderEnumDto? Provider = null,
    [property: JsonPropertyName("options")]
    ChatOptionsDto? Options = null);


public sealed record ConversationResponseDto(
    [property: JsonPropertyName("conversation_id")]
    string ConversationId,
    [property: JsonPropertyName("responses")]
    IList<ChatMessageDto> Responses,
    [property: JsonPropertyName("model_id")]
    string? ModelId,
    [property: JsonPropertyName("created_at")]
    DateTimeOffset? CreatedAt,
    [property: JsonPropertyName("finish_reason")]
    ChatFinishReasonDto? FinishReason);

public sealed record ConversationUpdateResponseDto(
    [property: JsonPropertyName("conversation_id")]
    string ConversationId,
    [property: JsonPropertyName("role")] ChatRoleEnumDto? Role,
    [property: JsonPropertyName("contents")]
    IList<AiContentDto> Contents,
    [property: JsonPropertyName("created_at")]
    DateTimeOffset? CreatedAt,
    [property: JsonPropertyName("finish_reason")]
    ChatFinishReasonDto? FinishReason,
    [property: JsonPropertyName("model_id")]
    string? ModelId
);



public sealed record ConversationDto(
    [property: JsonPropertyName("conversation_id")]
    string ConversationId,
    [property: JsonPropertyName("title")]
    string Title,
    [property: JsonPropertyName("description")]
    string? Description,
    [property: JsonPropertyName("messages")]
    IList<ChatMessageDto> Messages,
    [property: JsonPropertyName("created_at")]
    DateTimeOffset? CreatedAt,
    [property: JsonPropertyName("finish_reason")]
    ChatFinishReasonDto? FinishReason,
    [property: JsonPropertyName("chat_options")]
    ChatOptionsDto? ChatOptions,
    [property: JsonPropertyName("provider")]
    string? Provider
    );
