using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(NoneChatToolModeDto), "none")]
[JsonDerivedType(typeof(AutoChatToolModeDto), "auto")]
[JsonDerivedType(typeof(RequiredChatToolModeDto), "required")]
public abstract record ChatToolModeDto;
