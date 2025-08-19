namespace ElTocardo.Application.Commands.PresetChatOptions;

public sealed record CreatePresetChatOptionsCommand(
	string Name,
	string? ConversationId,
	string? Instructions,
	float? Temperature,
	int? MaxOutputTokens,
	float? TopP,
	int? TopK,
	float? FrequencyPenalty,
	float? PresencePenalty,
	long? Seed,
	string? ResponseFormat,
	string? ModelId,
	IList<string>? StopSequences,
	bool? AllowMultipleToolCalls,
	string? ToolMode,
	string? Tools,
	string? RequiredFunctionName
);
