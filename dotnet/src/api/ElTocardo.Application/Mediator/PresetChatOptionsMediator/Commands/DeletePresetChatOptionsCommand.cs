using ElTocardo.Domain.Mediator.Commands;

namespace ElTocardo.Application.Mediator.PresetChatOptionsMediator.Commands;

public record DeletePresetChatOptionsCommand(string Key) : DeleteCommandBase<string>(Key);
