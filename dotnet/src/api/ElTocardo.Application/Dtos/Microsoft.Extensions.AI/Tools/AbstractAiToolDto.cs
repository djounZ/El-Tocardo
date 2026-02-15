using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Tools;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(HostedFileSearchToolDto), "hosted_file_search")]
[JsonDerivedType(typeof(HostedWebSearchToolDto), "hosted_web_search")]
[JsonDerivedType(typeof(AiToolDto), "ai_tool")]
public abstract record AbstractAiToolDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description);

public record AiToolDto(
    string Name,
    string Description)
    : AbstractAiToolDto(Name, Description);
