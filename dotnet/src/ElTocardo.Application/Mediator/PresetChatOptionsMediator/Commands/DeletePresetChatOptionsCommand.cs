using ElTocardo.Application.Mediator.Common.Commands;

namespace ElTocardo.Application.Mediator.PresetChatOptionsMediator.Commands;

public record DeletePresetChatOptionsCommand(string Key) : DeleteCommandBase<string>(Key);
