using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Dtos.AI.ChatCompletion.Request;
using ElTocardo.Application.Services;
using ElTocardo.Domain.Entities;
using ElTocardo.Domain.Repositories;
using System.Text.Json;

namespace ElTocardo.Infrastructure.Services;

public class PresetChatOptionsService(IPresetChatOptionsRepository repository) : IPresetChatOptionsService
{
    public async Task<IEnumerable<PresetChatOptionsDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await repository.GetAllAsync(cancellationToken);
        return entities.Select(ToDto);
    }

    public async Task<PresetChatOptionsDto?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByNameAsync(name, cancellationToken);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<Guid> CreateAsync(PresetChatOptionsDto dto, CancellationToken cancellationToken = default)
    {
        var entity = FromDto(dto);
        entity.Id = Guid.NewGuid();
        await repository.AddAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(string name, PresetChatOptionsDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByNameAsync(name, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        var updated = FromDto(dto);
        entity.ConversationId = updated.ConversationId;
        entity.Instructions = updated.Instructions;
        entity.Temperature = updated.Temperature;
        entity.MaxOutputTokens = updated.MaxOutputTokens;
        entity.TopP = updated.TopP;
        entity.TopK = updated.TopK;
        entity.FrequencyPenalty = updated.FrequencyPenalty;
        entity.PresencePenalty = updated.PresencePenalty;
        entity.Seed = updated.Seed;
        entity.ResponseFormat = updated.ResponseFormat;
        entity.ModelId = updated.ModelId;
        entity.StopSequences = updated.StopSequences;
        entity.AllowMultipleToolCalls = updated.AllowMultipleToolCalls;
        entity.ToolMode = updated.ToolMode;
        entity.Tools = updated.Tools;
        await repository.UpdateAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(string name, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByNameAsync(name, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        await repository.DeleteAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static PresetChatOptionsDto ToDto(PresetChatOptions entity)
    {
        var chatOptions = new ChatOptionsDto(
            entity.ConversationId,
            entity.Instructions,
            entity.Temperature,
            entity.MaxOutputTokens,
            entity.TopP,
            entity.TopK,
            entity.FrequencyPenalty,
            entity.PresencePenalty,
            entity.Seed,
            entity.ResponseFormat is not null ? JsonSerializer.Deserialize<ChatResponseFormatDto>(entity.ResponseFormat) : null,
            entity.ModelId,
            entity.StopSequences?.Split(",").ToList(),
            entity.AllowMultipleToolCalls,
            entity.ToolMode is not null ? JsonSerializer.Deserialize<ChatToolModeDto>(entity.ToolMode) : null,
            entity.Tools is not null ? JsonSerializer.Deserialize<IDictionary<string, IList<AiToolDto>>>(entity.Tools) : null
        );
        return new PresetChatOptionsDto(entity.Name, chatOptions);
    }

    private static PresetChatOptions FromDto(PresetChatOptionsDto dto)
    {
        return new PresetChatOptions
        {
            Name = dto.Name,
            ConversationId = dto.ChatOptions.ConversationId,
            Instructions = dto.ChatOptions.Instructions,
            Temperature = dto.ChatOptions.Temperature,
            MaxOutputTokens = dto.ChatOptions.MaxOutputTokens,
            TopP = dto.ChatOptions.TopP,
            TopK = dto.ChatOptions.TopK,
            FrequencyPenalty = dto.ChatOptions.FrequencyPenalty,
            PresencePenalty = dto.ChatOptions.PresencePenalty,
            Seed = dto.ChatOptions.Seed,
            ResponseFormat = dto.ChatOptions.ResponseFormat is not null ? JsonSerializer.Serialize(dto.ChatOptions.ResponseFormat) : null,
            ModelId = dto.ChatOptions.ModelId,
            StopSequences = dto.ChatOptions.StopSequences is not null ? string.Join(",", dto.ChatOptions.StopSequences) : null,
            AllowMultipleToolCalls = dto.ChatOptions.AllowMultipleToolCalls,
            ToolMode = dto.ChatOptions.ToolMode?.ToString(),
            Tools = dto.ChatOptions.Tools is not null ? JsonSerializer.Serialize(dto.ChatOptions.Tools) : null
        };
    }
}
