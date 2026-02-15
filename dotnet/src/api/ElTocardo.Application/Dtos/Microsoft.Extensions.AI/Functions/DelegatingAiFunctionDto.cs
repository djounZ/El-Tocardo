namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Functions;

public record DelegatingAiFunctionDto(
    string Name,
    string Description,
    string Schema,
    string? ReturnJsonSchema,
    string? UnderlyingMethod): AiFunctionDto(Name, Description,Schema,ReturnJsonSchema, UnderlyingMethod);
