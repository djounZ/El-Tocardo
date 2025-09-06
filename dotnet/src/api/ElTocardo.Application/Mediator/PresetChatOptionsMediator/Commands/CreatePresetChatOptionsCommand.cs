
namespace ElTocardo.Application.Mediator.PresetChatOptionsMediator.Commands;

public record CreatePresetChatOptionsCommand(
    string Name,
    float? Temperature,
    int? MaxOutputTokens,
    float? TopP,
    int? TopK,
    float? FrequencyPenalty,
    float? PresencePenalty,
    long? Seed,
    string? ResponseFormat,
    string? StopSequences,
    bool? AllowMultipleToolCalls,
    string? ToolMode,
    string? Tools);
