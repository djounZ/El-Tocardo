using ElTocardo.Application.Dtos.ModelContextProtocol;

namespace ElTocardo.Application.Services;

public interface IMcpClientToolsService
{
    public Task<IDictionary<string, IList<McpClientToolDto>>> GetAll(CancellationToken cancellationToken);

    public Task<CallToolResultDto> CallToolAsync(McpClientToolRequestDto request,
        CancellationToken cancellationToken);
}
