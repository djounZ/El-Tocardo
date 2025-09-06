namespace ElTocardo.Application.Mediator.PresetChatInstructionMediator.Commands;

public sealed record UpdatePresetChatInstructionCommand(
	string Name,
	string Description,
	string ContentType,
	string Content);
