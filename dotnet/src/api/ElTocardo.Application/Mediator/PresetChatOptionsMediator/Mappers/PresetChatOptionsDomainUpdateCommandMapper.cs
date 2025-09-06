using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Commands;
using ElTocardo.Domain.Mediator.Common.Mappers;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;

namespace ElTocardo.Application.Mediator.PresetChatOptionsMediator.Mappers;

public class PresetChatOptionsDomainUpdateCommandMapper : AbstractDomainUpdateCommandMapper<PresetChatOptions, Guid,string, UpdatePresetChatOptionsCommand>
{
    public override void UpdateFromCommand(PresetChatOptions domain, UpdatePresetChatOptionsCommand command)
    {
        domain.Update(
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
