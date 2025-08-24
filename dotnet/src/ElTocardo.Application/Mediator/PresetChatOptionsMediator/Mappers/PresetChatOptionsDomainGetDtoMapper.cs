using System.Text.Json;
using ElTocardo.Application.Dtos.AI.ChatCompletion.Request;
using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Mediator.Common.Mappers;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;

namespace ElTocardo.Application.Mediator.PresetChatOptionsMediator.Mappers;

public class PresetChatOptionsDomainGetDtoMapper : AbstractDomainGetDtoMapper<PresetChatOptions,Guid, string, PresetChatOptionsDto>
{
    public override PresetChatOptionsDto MapDomainToDto(PresetChatOptions entity)
    {
        var chatOptions = new ChatOptionsDto(
            null,
            entity.Instructions,
            entity.Temperature,
            entity.MaxOutputTokens,
            entity.TopP,
            entity.TopK,
            entity.FrequencyPenalty,
            entity.PresencePenalty,
            entity.Seed,
            entity.ResponseFormat is not null
                ? JsonSerializer.Deserialize<ChatResponseFormatDto>(entity.ResponseFormat)
                : null,
            null,
            entity.StopSequences?.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList(),
            entity.AllowMultipleToolCalls,
            entity.ToolMode is not null ? JsonSerializer.Deserialize<ChatToolModeDto>(entity.ToolMode) : null,
            entity.Tools is not null
                ? JsonSerializer.Deserialize<IDictionary<string, IList<AiToolDto>>>(entity.Tools)
                : null
        );

        return new PresetChatOptionsDto(entity.Name, chatOptions);
    }
}
