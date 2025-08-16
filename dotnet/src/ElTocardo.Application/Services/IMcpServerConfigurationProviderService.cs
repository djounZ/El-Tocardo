using ElTocardo.Application.Dtos.ModelContextProtocol;

namespace ElTocardo.Application.Services;

public interface IMcpServerConfigurationProviderService
{
    public IDictionary<string, McpServerConfigurationItemDto> GetAllServers();
    public McpServerConfigurationItemDto? GetServer(string serverName);
    public bool CreateServer(string serverName, McpServerConfigurationItemDto item);
    public bool UpdateServer(string serverName, McpServerConfigurationItemDto item);
    public bool DeleteServer(string serverName);
}
