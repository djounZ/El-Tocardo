using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;

public sealed record RequiredChatToolModeDto(
    [property: JsonPropertyName("required_function_name")]
    string? RequiredFunctionName
) : ChatToolModeDto;
