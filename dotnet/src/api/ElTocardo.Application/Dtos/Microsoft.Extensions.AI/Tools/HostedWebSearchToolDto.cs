namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Tools;

public record HostedWebSearchToolDto(
    string Name,
    string Description)
    : AbstractAiToolDto(Name, Description);