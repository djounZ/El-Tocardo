using ElTocardo.Application.Common.Interfaces;
using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Queries.PresetChatOptions;
using ElTocardo.Application.Services;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Handlers.PresetChatOptions;

public class GetPresetChatOptionsByNameQueryHandler(
    ILogger<GetPresetChatOptionsByNameQueryHandler> logger,
    IPresetChatOptionsService service) : QueryHandlerBase<GetPresetChatOptionsByNameQuery, PresetChatOptionsDto>(logger)
{
    protected override async Task<PresetChatOptionsDto> HandleAsyncImplementation(GetPresetChatOptionsByNameQuery query,
        CancellationToken cancellationToken = default)
    {
        var presetChatOptionsDto = await service.GetByNameAsync(query.Name, cancellationToken);
        return presetChatOptionsDto!;
    }
}
