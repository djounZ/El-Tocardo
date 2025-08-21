using ElTocardo.Application.Mediator.Common.Mappers;
using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Commands;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;

namespace ElTocardo.Application.Mediator.PresetChatOptionsMediator.Mappers;

public class PresetChatOptionsDomainUpdateCommandMapper : AbstractDomainUpdateCommandMapper<PresetChatOptions, Guid,string, UpdatePresetChatOptionsCommand>
{
    public override void UpdateFromCommand(PresetChatOptions domain, UpdatePresetChatOptionsCommand command)
    {
        domain.Update(
            command.ConversationId,
            command.Instructions,
            command.Temperature,
            command.MaxOutputTokens,
            command.TopP,
            command.TopK,
            command.FrequencyPenalty,
            command.PresencePenalty,
            command.Seed,
            command.ResponseFormat,
            command.ModelId,
            command.StopSequences,
            command.AllowMultipleToolCalls,
            command.ToolMode,
            command.Tools);
    }
}
