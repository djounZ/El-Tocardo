using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Mediator.Common.Handlers.Queries;
using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Mappers;
using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Queries;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Repositories;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.PresetChatOptionsMediator.Handlers.Queries;

public class GetAllPresetChatOptionsQueryHandler(
    IPresetChatOptionsRepository repository,
    ILogger<GetAllPresetChatOptionsQueryHandler> logger,
    PresetChatOptionsDomainGetAllDtoMapper mapper)
    :  GetAllEntitiesQueryHandler<PresetChatOptions, Guid,string, GetAllPresetChatOptionsQuery,
        List<PresetChatOptionsDto>>(repository, logger, mapper);
