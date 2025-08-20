using System.Text.Json;
using ElTocardo.Application.Commands.PresetChatOptions;
using ElTocardo.Application.Common.Interfaces;
using ElTocardo.Application.Dtos.AI.ChatCompletion.Request;
using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Services;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Handlers.PresetChatOptions;

public class CreatePresetChatOptionsCommandHandler(
    ILogger<CreatePresetChatOptionsCommandHandler> logger,
    IPresetChatOptionsService service) : CommandHandlerBase<CreatePresetChatOptionsCommand, Guid>(logger)
{
    protected override async Task<Guid> HandleAsyncImplementation(CreatePresetChatOptionsCommand command,
        CancellationToken cancellationToken = default)
    {
        ChatToolModeDto? toolMode = command.ToolMode?.ToLowerInvariant() switch
        {
            "none" => new NoneChatToolModeDto(),
            "auto" => new AutoChatToolModeDto(),
            "required" => new RequiredChatToolModeDto(command.RequiredFunctionName),
            _ => null
        };
        var dto = new PresetChatOptionsDto(
            command.Name,
            new ChatOptionsDto(
                command.ConversationId,
                command.Instructions,
                command.Temperature,
                command.MaxOutputTokens,
                command.TopP,
                command.TopK,
                command.FrequencyPenalty,
                command.PresencePenalty,
                command.Seed,
                command.ResponseFormat is not null
                    ? JsonSerializer.Deserialize<ChatResponseFormatDto>(command.ResponseFormat)
                    : null,
                command.ModelId,
                command.StopSequences,
                command.AllowMultipleToolCalls,
                toolMode,
                command.Tools is not null
                    ? JsonSerializer.Deserialize<IDictionary<string, IList<AiToolDto>>>(command.Tools)
                    : null
            )
        );
        return await service.CreateAsync(dto, cancellationToken);
    }
}
