using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Commands;
using ElTocardo.Domain.Mediator.Common.Mappers;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;

namespace ElTocardo.Application.Mediator.PresetChatOptionsMediator.Mappers;

public class PresetChatOptionsDomainCreateCommandMapper : AbstractDomainCreateCommandMapper<PresetChatOptions, Guid,string, CreatePresetChatOptionsCommand>
{
    public override PresetChatOptions CreateFromCommand(CreatePresetChatOptionsCommand command)
    {
        return new PresetChatOptions(
            command.Name,
            command.Temperature,
            command.MaxOutputTokens,
            command.TopP,
            command.TopK,
            command.FrequencyPenalty,
            command.PresencePenalty,
            command.Seed,
            null, // todo
            command.ResponseFormat,
            command.StopSequences,
            command.AllowMultipleToolCalls,
            command.ToolMode,
            command.Tools);
    }
}
