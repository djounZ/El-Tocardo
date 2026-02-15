using System.Text.Json.Serialization;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Functions;

namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Tools;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(HostedFileSearchToolDto), "hosted_file_search")]
[JsonDerivedType(typeof(HostedWebSearchToolDto), "hosted_web_search")]
[JsonDerivedType(typeof(DelegatingAiFunctionDeclarationDto), "delegating_ai_function_declaration")]
[JsonDerivedType(typeof(DelegatingAiFunctionDto), "delegating_ai_function")]
[JsonDerivedType(typeof(AiToolDto), "ai_tool")]
public abstract record AbstractAiToolDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description);
