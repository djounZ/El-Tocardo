using ElTocardo.Application.Commands.PresetChatOptions;
using ElTocardo.Application.Common.Interfaces;
using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Services;

namespace ElTocardo.Application.Handlers.PresetChatOptions;

public class CreatePresetChatOptionsCommandHandler(IPresetChatOptionsService service) : ICommandHandler<CreatePresetChatOptionsCommand, Guid>
{
    public async Task<Guid> HandleAsync(CreatePresetChatOptionsCommand command, CancellationToken cancellationToken = default)
    {
        ElTocardo.Application.Dtos.AI.ChatCompletion.Request.ChatToolModeDto? toolMode = command.ToolMode?.ToLowerInvariant() switch
        {
            "none" => new ElTocardo.Application.Dtos.AI.ChatCompletion.Request.NoneChatToolModeDto(),
            "auto" => new ElTocardo.Application.Dtos.AI.ChatCompletion.Request.AutoChatToolModeDto(),
            "required" => new ElTocardo.Application.Dtos.AI.ChatCompletion.Request.RequiredChatToolModeDto(command.RequiredFunctionName),
            _ => null
        };
        var dto = new PresetChatOptionsDto(
            command.Name,
            new ElTocardo.Application.Dtos.AI.ChatCompletion.Request.ChatOptionsDto(
                command.ConversationId,
                command.Instructions,
                command.Temperature,
                command.MaxOutputTokens,
                command.TopP,
                command.TopK,
                command.FrequencyPenalty,
                command.PresencePenalty,
                command.Seed,
                command.ResponseFormat is not null ? System.Text.Json.JsonSerializer.Deserialize<ElTocardo.Application.Dtos.AI.ChatCompletion.Request.ChatResponseFormatDto>(command.ResponseFormat) : null,
                command.ModelId,
                command.StopSequences,
                command.AllowMultipleToolCalls,
                toolMode,
                command.Tools is not null ? System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.IDictionary<string, System.Collections.Generic.IList<ElTocardo.Application.Dtos.AI.ChatCompletion.Request.AiToolDto>>>(command.Tools) : null
            )
        );
        return await service.CreateAsync(dto, cancellationToken);
    }
}
