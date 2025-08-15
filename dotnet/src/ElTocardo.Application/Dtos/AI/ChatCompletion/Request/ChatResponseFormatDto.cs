using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.AI.ChatCompletion.Request;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(ChatResponseFormatTextDto), "text")]
[JsonDerivedType(typeof(ChatResponseFormatJsonDto), "json")]
public abstract record ChatResponseFormatDto;
