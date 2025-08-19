using ElTocardo.Domain.ValueObjects;

namespace ElTocardo.Application.Commands.McpServerConfiguration;

public sealed record UpdateMcpServerCommand(
    string ServerName,
    string? Category,
    string? Command,
    IList<string>? Arguments,
    IDictionary<string, string?>? EnvironmentVariables,
    Uri? Endpoint,
    McpServerTransportType TransportType);
