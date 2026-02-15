using System.Text.Json;
using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Tools;
using ElTocardo.Domain.Mediator.Common.Mappers;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;

namespace ElTocardo.Application.Mediator.PresetChatOptionsMediator.Mappers;

public class PresetChatOptionsDomainGetDtoMapper : AbstractDomainGetDtoMapper<PresetChatOptions,Guid, string, PresetChatOptionsDto>
{
    public override PresetChatOptionsDto MapDomainToDto(PresetChatOptions entity)
    {
        var chatOptions = new ChatOptionsDto(
            null,
            null,
            entity.Temperature,
            entity.MaxOutputTokens,
            entity.TopP,
            entity.TopK,
            entity.FrequencyPenalty,
            entity.PresencePenalty,
            entity.Seed,
            null,// todo entity.Reasoning?.ToString(),
            entity.ResponseFormat is not null
                ? JsonSerializer.Deserialize<ChatResponseFormatDto>(entity.ResponseFormat)
                : null,
            null,
            entity.StopSequences?.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList(),
            entity.AllowMultipleToolCalls,
            entity.ToolMode is not null ? JsonSerializer.Deserialize<ChatToolModeDto>(entity.ToolMode) : null,
            entity.Tools is not null
                ? JsonSerializer.Deserialize<IDictionary<string, IList<AbstractAiToolDto>>>(entity.Tools)
                : null
        );

        return new PresetChatOptionsDto(entity.Name, chatOptions);
    }
}
