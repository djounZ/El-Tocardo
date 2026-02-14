using System.Text.Json.Serialization;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;

namespace ElTocardo.Application.Dtos.Configuration;

public sealed record  PresetChatOptionsDto (
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("chat_options")]
    ChatOptionsDto ChatOptions);
