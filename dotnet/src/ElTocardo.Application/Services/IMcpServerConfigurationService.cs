using ElTocardo.Application.Commands.McpServerConfiguration;
using ElTocardo.Application.Common.Models;
using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Queries.McpServerConfiguration;
using ElTocardo.Domain.ValueObjects;

namespace ElTocardo.Application.Services;

public interface IMcpServerConfigurationService
{
    public Task<IDictionary<string, McpServerConfigurationItemDto>> GetAllServersAsync(CancellationToken cancellationToken = default);
    public Task<McpServerConfigurationItemDto?> GetServerAsync(string serverName, CancellationToken cancellationToken = default);
    public Task<Result<Guid>> CreateServerAsync(string serverName, McpServerConfigurationItemDto item, CancellationToken cancellationToken = default);
    public Task<VoidResult> UpdateServerAsync(string serverName, McpServerConfigurationItemDto item, CancellationToken cancellationToken = default);
    public Task<VoidResult> DeleteServerAsync(string serverName, CancellationToken cancellationToken = default);
}
