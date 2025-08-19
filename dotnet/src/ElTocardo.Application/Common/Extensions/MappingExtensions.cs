using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Domain.ValueObjects;

namespace ElTocardo.Application.Common.Extensions;

public static class MappingExtensions
{
    public static McpServerTransportTypeDto ToDto(this McpServerTransportType transportType)
    {
        return transportType switch
        {
            McpServerTransportType.Stdio => McpServerTransportTypeDto.Stdio,
            McpServerTransportType.Http => McpServerTransportTypeDto.Http,
            _ => throw new ArgumentOutOfRangeException(nameof(transportType), transportType, null)
        };
    }

    public static McpServerTransportType ToDomain(this McpServerTransportTypeDto transportType)
    {
        return transportType switch
        {
            McpServerTransportTypeDto.Stdio => McpServerTransportType.Stdio,
            McpServerTransportTypeDto.Http => McpServerTransportType.Http,
            _ => throw new ArgumentOutOfRangeException(nameof(transportType), transportType, null)
        };
    }
}
