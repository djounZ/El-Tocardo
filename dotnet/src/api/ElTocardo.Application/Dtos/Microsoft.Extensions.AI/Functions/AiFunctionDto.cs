using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Functions;

public abstract record AiFunctionDto(
    string Name,
    string Description,
    string Schema,
    string? ReturnJsonSchema,
    [property: JsonPropertyName("underlying_method")] string? UnderlyingMethod): AiFunctionDeclarationDto(Name, Description,Schema,ReturnJsonSchema);
