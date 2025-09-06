using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.AI.ChatCompletion.Request;

public sealed record RequiredChatToolModeDto(
    [property: JsonPropertyName("required_function_name")]
    string? RequiredFunctionName
) : ChatToolModeDto;
