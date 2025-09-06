using ElTocardo.Domain.Mediator.Common.Commands;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.ValueObjects;

namespace ElTocardo.Application.Mediator.McpServerConfigurationMediator.Commands;

public sealed record UpdateMcpServerCommand(
    string Id,
    string? Category,
    string? Command,
    IList<string>? Arguments,
    IDictionary<string, string?>? EnvironmentVariables,
    Uri? Endpoint,
    McpServerTransportType TransportType) : UpdateCommandBase<string>(Id);
