using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Dtos.ModelContextProtocol.Core;
using ElTocardo.Domain.Models;

namespace ElTocardo.Application.Services;

public interface IMcpServerConfigurationEndpointService
{
    public Task<Result<Dictionary<string, McpServerConfigurationItemDto>>> GetAllServersAsync(
        CancellationToken cancellationToken = default);

    public Task<Result<McpServerConfigurationItemDto>> GetServerAsync(string serverName,
        CancellationToken cancellationToken = default);

    public Task<VoidResult> CreateServerAsync(string serverName, McpServerConfigurationItemDto item,
        CancellationToken cancellationToken = default);

    public Task<VoidResult> UpdateServerAsync(string serverName, McpServerConfigurationItemDto item,
        CancellationToken cancellationToken = default);

    public Task<VoidResult> DeleteServerAsync(string serverName, CancellationToken cancellationToken = default);
}
