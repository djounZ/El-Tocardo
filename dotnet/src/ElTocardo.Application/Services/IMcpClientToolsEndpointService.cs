using ElTocardo.Application.Dtos.ModelContextProtocol;

namespace ElTocardo.Application.Services;

public interface IMcpClientToolsEndpointService
{
    public Task<IDictionary<string, IList<McpClientToolDto>>> GetAll(CancellationToken cancellationToken);

    public Task<CallToolResultDto> CallToolAsync(McpClientToolRequestDto request,
        CancellationToken cancellationToken);
}
