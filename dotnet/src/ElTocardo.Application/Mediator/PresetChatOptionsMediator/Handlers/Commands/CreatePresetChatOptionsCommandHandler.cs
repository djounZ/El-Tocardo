using ElTocardo.Application.Mediator.Common.Handlers.Commands;
using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Commands;
using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Mappers;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.PresetChatOptionsMediator.Handlers.Commands;

public class CreatePresetChatOptionsCommandHandler(
    IPresetChatOptionsRepository repository,
    ILogger<CreatePresetChatOptionsCommandHandler> logger,
    IValidator<CreatePresetChatOptionsCommand> validator,
    PresetChatOptionsDomainCreateCommandMapper mapper)
    : CreateEntityCommandHandler<PresetChatOptions, string, CreatePresetChatOptionsCommand>(repository, logger, validator, mapper);
