using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Infrastructure.Mappers.Dtos.ModelContextProtocol;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;

namespace ElTocardo.Infrastructure.Services;

public class ClientTransportFactoryService(ILogger<ClientTransportFactoryService> logger, ModelContextProtocolMapper mapper,  ILoggerFactory? loggerFactory)
{


    public IClientTransport Create(McpServerConfigurationItemDto configurationItem)
    {
        if (configurationItem.Type is McpServerTransportTypeDto.Stdio)
        {
            return new StdioClientTransport(mapper.MapToStdioClientTransportOptions(configurationItem), loggerFactory);
        }

        if (configurationItem.Type is McpServerTransportTypeDto.Http)
        {
            return new SseClientTransport(mapper.MapToSseClientTransportOptions(configurationItem), loggerFactory);
        }

        logger.LogError("Unsupported configuration type: {ConfigurationType}", configurationItem.GetType().Name);
        throw new NotSupportedException($"Unsupported configuration type: {configurationItem.GetType().Name}");
    }

}
