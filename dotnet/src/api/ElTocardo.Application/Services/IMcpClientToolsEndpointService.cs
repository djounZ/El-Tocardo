using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Dtos.ModelContextProtocol.Core;
using ElTocardo.Application.Dtos.ModelContextProtocol.Core.Client;
using ElTocardo.Application.Dtos.ModelContextProtocol.Core.Protocol;
using ElTocardo.Domain.Models;

namespace ElTocardo.Application.Services;

public interface IMcpClientToolsEndpointService
{
    public Task<Result<IDictionary<string, IList<McpClientToolDto>>>> GetAll(CancellationToken cancellationToken);

    public Task<Result<CallToolResultDto>> CallToolAsync(McpClientToolRequestDto request,
        CancellationToken cancellationToken);
}
