using ElTocardo.Domain.ValueObjects;

namespace ElTocardo.Application.Commands.McpServerConfiguration;

public record CreateMcpServerCommand(
    string ServerName,
    string? Category,
    string? Command,
    IList<string>? Arguments,
    IDictionary<string, string?>? EnvironmentVariables,
    Uri? Endpoint,
    McpServerTransportType TransportType);
