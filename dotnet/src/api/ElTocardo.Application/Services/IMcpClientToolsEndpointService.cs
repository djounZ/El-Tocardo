using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Domain.Models;

namespace ElTocardo.Application.Services;

public interface IMcpClientToolsEndpointService
{
    public Task<Result<IDictionary<string, IList<McpClientToolDto>>>> GetAll(CancellationToken cancellationToken);

    public Task<Result<CallToolResultDto>> CallToolAsync(McpClientToolRequestDto request,
        CancellationToken cancellationToken);
}
