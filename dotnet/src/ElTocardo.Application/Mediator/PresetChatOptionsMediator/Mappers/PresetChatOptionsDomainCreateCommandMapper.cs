using ElTocardo.Application.Mediator.Common.Mappers;
using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Commands;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;

namespace ElTocardo.Application.Mediator.PresetChatOptionsMediator.Mappers;

public class PresetChatOptionsDomainCreateCommandMapper : AbstractDomainCreateCommandMapper<PresetChatOptions, Guid,string, CreatePresetChatOptionsCommand>
{
    public override PresetChatOptions CreateFromCommand(CreatePresetChatOptionsCommand command)
    {
        return new PresetChatOptions(
            command.Name,
            command.Instructions,
            command.Temperature,
            command.MaxOutputTokens,
            command.TopP,
            command.TopK,
            command.FrequencyPenalty,
            command.PresencePenalty,
            command.Seed,
            command.ResponseFormat,
            command.StopSequences,
            command.AllowMultipleToolCalls,
            command.ToolMode,
            command.Tools);
    }
}
