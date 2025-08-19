using ElTocardo.Application.Commands.PresetChatOptions;
using ElTocardo.Application.Common.Interfaces;
using ElTocardo.Application.Services;

namespace ElTocardo.Application.Handlers.PresetChatOptions;

public class DeletePresetChatOptionsCommandHandler(IPresetChatOptionsService service) : ICommandHandler<DeletePresetChatOptionsCommand, bool>
{
    public async Task<bool> HandleAsync(DeletePresetChatOptionsCommand command, CancellationToken cancellationToken = default)
        => await service.DeleteAsync(command.Name, cancellationToken);
}