using ElTocardo.Application.Mediator.Common.Commands;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.ValueObjects;

namespace ElTocardo.Application.Mediator.McpServerConfigurationMediator.Commands;

public sealed record UpdateMcpServerCommand(
    string Key,
    string? Category,
    string? Command,
    IList<string>? Arguments,
    IDictionary<string, string?>? EnvironmentVariables,
    Uri? Endpoint,
    McpServerTransportType TransportType) : UpdateCommandBase<string>(Key);
