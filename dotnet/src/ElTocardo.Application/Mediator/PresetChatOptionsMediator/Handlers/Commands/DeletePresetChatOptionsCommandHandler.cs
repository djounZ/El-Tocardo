using ElTocardo.Application.Mediator.Common.Handlers.Commands;
using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Commands;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Repositories;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.PresetChatOptionsMediator.Handlers.Commands;

public class DeletePresetChatOptionsCommandHandler(
    IPresetChatOptionsRepository repository,
    ILogger<DeletePresetChatOptionsCommandHandler> logger)
    : DeleteEntityCommandHandler<PresetChatOptions, string, DeletePresetChatOptionsCommand>(repository, logger);
