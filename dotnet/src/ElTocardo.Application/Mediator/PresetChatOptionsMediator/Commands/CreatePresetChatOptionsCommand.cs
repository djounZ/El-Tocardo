
namespace ElTocardo.Application.Mediator.PresetChatOptionsMediator.Commands;

public record CreatePresetChatOptionsCommand(
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
    string? StopSequences,
    bool? AllowMultipleToolCalls,
    string? ToolMode,
    string? Tools);
