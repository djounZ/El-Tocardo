namespace ElTocardo.Application.Mediator.PresetChatInstructionMediator.Commands;

public sealed record CreatePresetChatInstructionCommand(
	string Name,
	string Description,
	string ContentType,
	string Content);
