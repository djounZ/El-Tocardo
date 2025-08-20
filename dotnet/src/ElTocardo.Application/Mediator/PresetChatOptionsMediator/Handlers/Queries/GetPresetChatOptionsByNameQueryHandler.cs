using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Mediator.Common.Handlers.Queries;
using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Mappers;
using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Queries;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Repositories;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.PresetChatOptionsMediator.Handlers.Queries;

public class GetPresetChatOptionsByNameQueryHandler(
    IPresetChatOptionsRepository repository,
    ILogger<GetPresetChatOptionsByNameQueryHandler> logger,
    PresetChatOptionsDomainGetDtoMapper mapper)
    : GetEntityByKeyQueryHandler<PresetChatOptions, string, GetPresetChatOptionsByNameQuery,
        PresetChatOptionsDto>(repository, logger, mapper);
