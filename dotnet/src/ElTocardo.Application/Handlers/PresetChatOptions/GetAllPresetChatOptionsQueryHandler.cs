using ElTocardo.Application.Common.Interfaces;
using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Queries.PresetChatOptions;
using ElTocardo.Application.Services;

namespace ElTocardo.Application.Handlers.PresetChatOptions;

public class GetAllPresetChatOptionsQueryHandler(IPresetChatOptionsService service) : IQueryHandler<GetAllPresetChatOptionsQuery, IEnumerable<PresetChatOptionsDto>>
{
    public async Task<IEnumerable<PresetChatOptionsDto>> HandleAsync(GetAllPresetChatOptionsQuery query, CancellationToken cancellationToken = default)
        => await service.GetAllAsync(cancellationToken);
}