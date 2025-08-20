using ElTocardo.Application.Common.Interfaces;
using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Queries.PresetChatOptions;
using ElTocardo.Application.Services;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Handlers.PresetChatOptions;

public class GetAllPresetChatOptionsQueryHandler(
    ILogger<GetAllPresetChatOptionsQueryHandler> logger,
    IPresetChatOptionsService service)
    : QueryHandlerBase<GetAllPresetChatOptionsQuery, List<PresetChatOptionsDto>>(logger)
{
    protected override async Task<List<PresetChatOptionsDto>> HandleAsyncImplementation(
        GetAllPresetChatOptionsQuery query, CancellationToken cancellationToken = default)
    {
        return await service.GetAllAsync(cancellationToken);
    }
}
