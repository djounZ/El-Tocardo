using ElTocardo.Application.Mediator.Common.Handlers.Commands;
using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Commands;
using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Mappers;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.PresetChatOptionsMediator.Handlers.Commands;

public class UpdatePresetChatOptionsCommandByKeyHandler(
    IPresetChatOptionsRepository repository,
    ILogger<UpdatePresetChatOptionsCommandByKeyHandler> logger,
    IValidator<UpdatePresetChatOptionsCommand> validator,
    PresetChatOptionsDomainUpdateCommandMapper mapper)
    : UpdateEntityCommandByKeyHandler<PresetChatOptions,Guid, string, UpdatePresetChatOptionsCommand>(repository, logger, validator, mapper);
