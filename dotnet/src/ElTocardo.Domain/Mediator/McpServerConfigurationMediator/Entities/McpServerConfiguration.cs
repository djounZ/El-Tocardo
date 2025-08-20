using ElTocardo.Domain.Mediator.Common.Entities;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.ValueObjects;

namespace ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;

public class McpServerConfiguration : AbstractEntity<string>
{
    private McpServerConfiguration() { } // EF Core constructor

    public McpServerConfiguration(
        string serverName,
        string? category,
        string? command,
        IList<string>? arguments,
        IDictionary<string, string?>? environmentVariables,
        Uri? endpoint,
        McpServerTransportType transportType)
    {
        if (string.IsNullOrWhiteSpace(serverName))
        {
            throw new ArgumentException("Server name cannot be null or empty", nameof(serverName));
        }

        Id = Guid.NewGuid();
        ServerName = serverName;
        Category = category;
        Command = command;
        Arguments = arguments ?? new List<string>();
        EnvironmentVariables = environmentVariables ?? new Dictionary<string, string?>();
        Endpoint = endpoint;
        TransportType = transportType;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        ValidateConfiguration();
    }

    public string ServerName { get; private set; } = string.Empty;
    public string? Category { get; private set; }
    public string? Command { get; private set; }
    public IList<string> Arguments { get; private set; } = new List<string>();
    public IDictionary<string, string?> EnvironmentVariables { get; private set; } = new Dictionary<string, string?>();
    public Uri? Endpoint { get; private set; }
    public McpServerTransportType TransportType { get; private set; }

    public void Update(
        string? category,
        string? command,
        IList<string>? arguments,
        IDictionary<string, string?>? environmentVariables,
        Uri? endpoint,
        McpServerTransportType transportType)
    {
        Category = category;
        Command = command;
        Arguments = arguments ?? new List<string>();
        EnvironmentVariables = environmentVariables ?? new Dictionary<string, string?>();
        Endpoint = endpoint;
        TransportType = transportType;
        UpdatedAt = DateTime.UtcNow;

        ValidateConfiguration();
    }

    private void ValidateConfiguration()
    {
        switch (TransportType)
        {
            case McpServerTransportType.Stdio when string.IsNullOrWhiteSpace(Command):
                throw new InvalidOperationException("Command is required for Stdio transport type");
            case McpServerTransportType.Http when Endpoint == null:
                throw new InvalidOperationException("Endpoint is required for Http transport type");
        }
    }

    public override string GetKey()
    {
        return ServerName;
    }
}
