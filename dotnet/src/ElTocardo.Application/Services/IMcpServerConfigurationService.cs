using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Mediator.Common.Models;

namespace ElTocardo.Application.Services;

public interface IMcpServerConfigurationService
{
    public Task<Result<IDictionary<string, McpServerConfigurationItemDto>>> GetAllServersAsync(
        CancellationToken cancellationToken = default);

    public Task<Result<McpServerConfigurationItemDto>> GetServerAsync(string serverName,
        CancellationToken cancellationToken = default);

    public Task<Result<Guid>> CreateServerAsync(string serverName, McpServerConfigurationItemDto item,
        CancellationToken cancellationToken = default);

    public Task<VoidResult> UpdateServerAsync(string serverName, McpServerConfigurationItemDto item,
        CancellationToken cancellationToken = default);

    public Task<VoidResult> DeleteServerAsync(string serverName, CancellationToken cancellationToken = default);
}
