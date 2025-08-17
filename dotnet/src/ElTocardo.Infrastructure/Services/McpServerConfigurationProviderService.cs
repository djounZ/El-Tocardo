using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Services;

namespace ElTocardo.Infrastructure.Services;

public sealed class McpServerConfigurationProviderService(McpServerConfigurationDto mcpServerConfiguration): IMcpServerConfigurationProviderService
{


    // CRUD methods for Servers property
    public IDictionary<string, McpServerConfigurationItemDto> GetAllServers()
    {
        return mcpServerConfiguration.Servers;
    }

    public McpServerConfigurationItemDto? GetServer(string serverName)
    {
        return mcpServerConfiguration.Servers.TryGetValue(serverName, out var item) ? item : null;
    }

    public bool CreateServer(string serverName, McpServerConfigurationItemDto item)
    {
        return mcpServerConfiguration.Servers.TryAdd(serverName, item);
    }

    public bool UpdateServer(string serverName, McpServerConfigurationItemDto item)
    {
        if (!mcpServerConfiguration.Servers.ContainsKey(serverName))
        {
            return false;
        }

        mcpServerConfiguration.Servers[serverName] = item;
        return true;
    }

    public bool DeleteServer(string serverName)
    {
        return mcpServerConfiguration.Servers.Remove(serverName);
    }
}
