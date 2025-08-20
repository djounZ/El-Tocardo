using ElTocardo.Application.Commands.PresetChatOptions;
using ElTocardo.Application.Mediator.Common.Interfaces;
using ElTocardo.Application.Services;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Handlers.PresetChatOptions;

public class DeletePresetChatOptionsCommandHandler(
    ILogger<DeletePresetChatOptionsCommandHandler> logger,
    IPresetChatOptionsService service) : CommandHandlerBase<DeletePresetChatOptionsCommand, bool>(logger)
{
    protected override async Task<bool> HandleAsyncImplementation(DeletePresetChatOptionsCommand command,
        CancellationToken cancellationToken = default)
    {
        return await service.DeleteAsync(command.Name, cancellationToken);
    }
}
