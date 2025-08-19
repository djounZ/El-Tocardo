using ElTocardo.Application.Common.Interfaces;
using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Queries.PresetChatOptions;
using ElTocardo.Application.Services;

namespace ElTocardo.Application.Handlers.PresetChatOptions;

public class GetPresetChatOptionsByNameQueryHandler(IPresetChatOptionsService service) : IQueryHandler<GetPresetChatOptionsByNameQuery, PresetChatOptionsDto?>
{
    public async Task<PresetChatOptionsDto?> HandleAsync(GetPresetChatOptionsByNameQuery query, CancellationToken cancellationToken = default)
        => await service.GetByNameAsync(query.Name, cancellationToken);
}