namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Tools;

public record AiToolDto(
    string Name,
    string Description)
    : AbstractAiToolDto(Name, Description);
