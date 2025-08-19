using ElTocardo.Domain.Entities;

namespace ElTocardo.Domain.Repositories;

public interface IMcpServerConfigurationRepository
{
    public Task<IEnumerable<McpServerConfiguration>> GetAllAsync(CancellationToken cancellationToken = default);
    public Task<McpServerConfiguration?> GetByNameAsync(string serverName, CancellationToken cancellationToken = default);
    public Task<McpServerConfiguration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public Task<bool> ExistsAsync(string serverName, CancellationToken cancellationToken = default);
    public Task AddAsync(McpServerConfiguration configuration, CancellationToken cancellationToken = default);
    public Task UpdateAsync(McpServerConfiguration configuration, CancellationToken cancellationToken = default);
    public Task DeleteAsync(McpServerConfiguration configuration, CancellationToken cancellationToken = default);
    public Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
