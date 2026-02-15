namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Functions;

public record DelegatingAiFunctionDeclarationDto(
    string Name,
    string Description,
    string Schema,
    string? ReturnJsonSchema): AiFunctionDeclarationDto(Name, Description, Schema, ReturnJsonSchema);
