using System.Text.Json.Serialization;
using ElTocardo.Application.Dtos.AI.ChatCompletion.Request;

namespace ElTocardo.Application.Dtos.Configuration;

public sealed record  PresetChatOptionsDto (
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("chat_options")]
    ChatOptionsDto ChatOptions);
