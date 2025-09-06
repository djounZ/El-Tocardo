using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.ValueObjects;

namespace ElTocardo.Application.Mediator.McpServerConfigurationMediator.Commands;

public sealed record CreateMcpServerCommand(
    string ServerName,
    string? Category,
    string? Command,
    IList<string>? Arguments,
    IDictionary<string, string?>? EnvironmentVariables,
    Uri? Endpoint,
    McpServerTransportType TransportType);
