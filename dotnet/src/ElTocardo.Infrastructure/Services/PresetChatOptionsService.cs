using System.Text.Json;
using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Mediator.Common.Interfaces;
using ElTocardo.Application.Mediator.Common.Models;
using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Commands;
using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Queries;
using ElTocardo.Application.Services;

namespace ElTocardo.Infrastructure.Services;

public class PresetChatOptionsService(
    IQueryHandler<GetAllPresetChatOptionsQuery, List<PresetChatOptionsDto>> getAllQueryHandler,
    IQueryHandler<GetPresetChatOptionsByNameQuery, PresetChatOptionsDto> getByNameQueryHandler,
    ICommandHandler<CreatePresetChatOptionsCommand, Guid> createCommandHandler,
    ICommandHandler<UpdatePresetChatOptionsCommand> updateCommandHandler,
    ICommandHandler<DeletePresetChatOptionsCommand> deleteCommandHandler)
    : IPresetChatOptionsService
{
    public async Task<Result<List<PresetChatOptionsDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await getAllQueryHandler.HandleAsync(GetAllPresetChatOptionsQuery.Instance, cancellationToken);
    }

    public async Task<Result<PresetChatOptionsDto>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await getByNameQueryHandler.HandleAsync(new GetPresetChatOptionsByNameQuery(name), cancellationToken);
    }

    public async Task<Result<Guid>> CreateAsync(PresetChatOptionsDto dto, CancellationToken cancellationToken = default)
    {
        var command = new CreatePresetChatOptionsCommand(
            dto.Name,
            dto.ChatOptions.Instructions,
            dto.ChatOptions.Temperature,
            dto.ChatOptions.MaxOutputTokens,
            dto.ChatOptions.TopP,
            dto.ChatOptions.TopK,
            dto.ChatOptions.FrequencyPenalty,
            dto.ChatOptions.PresencePenalty,
            dto.ChatOptions.Seed,
            dto.ChatOptions.ResponseFormat is not null
                ? JsonSerializer.Serialize(dto.ChatOptions.ResponseFormat)
                : null,
            dto.ChatOptions.StopSequences is not null ? string.Join(",", dto.ChatOptions.StopSequences) : null,
            dto.ChatOptions.AllowMultipleToolCalls,
            dto.ChatOptions.ToolMode?.ToString(),
            dto.ChatOptions.Tools is not null ? JsonSerializer.Serialize(dto.ChatOptions.Tools) : null
        );

        return await createCommandHandler.HandleAsync(command, cancellationToken);
    }

    public async Task<VoidResult> UpdateAsync(string name, PresetChatOptionsDto dto, CancellationToken cancellationToken = default)
    {
        var command = new UpdatePresetChatOptionsCommand(
            name,
            dto.ChatOptions.ConversationId,
            dto.ChatOptions.Instructions,
            dto.ChatOptions.Temperature,
            dto.ChatOptions.MaxOutputTokens,
            dto.ChatOptions.TopP,
            dto.ChatOptions.TopK,
            dto.ChatOptions.FrequencyPenalty,
            dto.ChatOptions.PresencePenalty,
            dto.ChatOptions.Seed,
            dto.ChatOptions.ResponseFormat is not null
                ? JsonSerializer.Serialize(dto.ChatOptions.ResponseFormat)
                : null,
            dto.ChatOptions.ModelId,
            dto.ChatOptions.StopSequences is not null ? string.Join(",", dto.ChatOptions.StopSequences) : null,
            dto.ChatOptions.AllowMultipleToolCalls,
            dto.ChatOptions.ToolMode?.ToString(),
            dto.ChatOptions.Tools is not null ? JsonSerializer.Serialize(dto.ChatOptions.Tools) : null
        );

        return await updateCommandHandler.HandleAsync(command, cancellationToken);
    }

    public async Task<VoidResult> DeleteAsync(string name, CancellationToken cancellationToken = default)
    {
        var command = new DeletePresetChatOptionsCommand(name);
        return await deleteCommandHandler.HandleAsync(command, cancellationToken);
    }
}
