using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(ChatResponseFormatTextDto), "text")]
[JsonDerivedType(typeof(ChatResponseFormatJsonDto), "json")]
public abstract record ChatResponseFormatDto;
