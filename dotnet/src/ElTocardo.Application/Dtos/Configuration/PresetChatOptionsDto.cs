using ElTocardo.Application.Dtos.AI.ChatCompletion.Request;

namespace ElTocardo.Application.Dtos.Configuration;

public sealed record  PresetChatOptionsDto (string Name, ChatOptionsDto ChatOptions);
