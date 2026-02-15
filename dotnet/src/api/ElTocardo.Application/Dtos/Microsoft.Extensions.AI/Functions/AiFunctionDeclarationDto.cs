using System.Text.Json.Serialization;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Tools;

namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Functions;

public abstract record AiFunctionDeclarationDto(
    string Name,
    string Description,
    [property: JsonPropertyName("schema")] string Schema,
    [property: JsonPropertyName("return_json_schema")] string? ReturnJsonSchema): AbstractAiToolDto(Name, Description);
