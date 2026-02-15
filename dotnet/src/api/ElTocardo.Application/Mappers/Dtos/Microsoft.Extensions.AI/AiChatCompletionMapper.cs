using System.Text.Json;
using ElTocardo.Application.Dtos.ChatCompletion;
using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Tools;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI;

public sealed class AiChatCompletionMapper(ILogger<AiChatCompletionMapper> logger, AiContentMapperOld aiContentMapper)
{
    public AiChatClientRequest MapToAiChatClientRequest(ChatRequestDto chatRequestDto)
    {
        logger.LogTrace("Mapping To AiChatClientRequest from ChatRequestDto {@ChatRequestDto}", chatRequestDto);
        return new AiChatClientRequest(
            [.. chatRequestDto.Messages.Select(MapToChatMessage)],
            MapToChatOptions(chatRequestDto.Options));
    }

    public ChatRequestDto MapToChatClientRequestDto(AiChatClientRequest aiChatClientRequest, AiProviderEnumDto provider)
    {
        logger.LogTrace("Mapping To ChatRequestDto from AiChatClientRequest {@AiChatClientRequest}",
            aiChatClientRequest);
        return new ChatRequestDto(
            [.. aiChatClientRequest.Messages.Select(MapToChatMessageDto)],
            provider,
            MapToChatOptionsDto(aiChatClientRequest.Options));
    }

    public ChatResponse MapToAiChatClientRequest(ChatResponseDto chatResponseDto)
    {
        logger.LogTrace("Mapping To ChatResponse from ChatResponseDto {@ChatRequestDto}", chatResponseDto);
        var chatResponse = new ChatResponse([.. chatResponseDto.Messages.Select(MapToChatMessage)])
        {
            ResponseId = chatResponseDto.ResponseId,
            ConversationId = chatResponseDto.ConversationId,
            ModelId = chatResponseDto.ModelId,
            CreatedAt = chatResponseDto.CreatedAt,
            FinishReason = MapToFinishReason(chatResponseDto.FinishReason)
        };
        return chatResponse;
    }

    public ChatResponseDto MapToChatResponseDto(ChatResponse chatResponse)
    {
        logger.LogTrace("Mapping To ChatResponseDto from ChatResponse {@ChatRequestDto}", chatResponse);
        var chatResponseDto = new ChatResponseDto(
            [.. chatResponse.Messages.Select(MapToChatMessageDto)],
            chatResponse.ResponseId,
            chatResponse.ConversationId,
            chatResponse.ModelId,
            chatResponse.CreatedAt,
            MapToFinishReasonDto(chatResponse.FinishReason),
            null
        );
        return chatResponseDto;
    }

    public ChatResponseUpdate MapToChatResponseUpdate(ChatResponseUpdateDto dto)
    {
        logger.LogTrace("Mapping To ChatResponse from ChatResponseDto {@ChatRequestDto}", dto);
        return new ChatResponseUpdate
        {
            AuthorName = dto.AuthorName,
            Role = dto.Role == null ? ChatRole.Assistant : MapToChatRole(dto.Role.Value),
            Contents = [.. dto.Contents.Select(aiContentMapper.MapToAiContent)],
            ResponseId = dto.ResponseId,
            MessageId = dto.MessageId,
            ConversationId = dto.ConversationId,
            CreatedAt = dto.CreatedAt,
            FinishReason = MapToFinishReason(dto.FinishReason),
            ModelId = dto.ModelId
        };
    }

    public ChatResponseUpdateDto MapToChatResponseUpdateDto(ChatResponseUpdate api)
    {
        logger.LogTrace("Mapping To ChatResponseUpdateDto from ChatResponseUpdate {@ChatRequestDto}", api);
        return new ChatResponseUpdateDto
        (
            api.AuthorName,
            api.Role == null ? null : MapToChatRoleDto(api.Role.Value),
            [.. api.Contents.Select(aiContentMapper.MapToAiContentDto)],
            api.ResponseId,
            api.MessageId,
            api.ConversationId,
            api.CreatedAt,
            MapToFinishReasonDto(api.FinishReason),
            api.ModelId
        );
    }

    private ChatFinishReason? MapToFinishReason(ChatFinishReasonDto? appModel)
    {
        return appModel switch
        {
            ChatFinishReasonDto.Stop => ChatFinishReason.Stop,
            ChatFinishReasonDto.Length => ChatFinishReason.Length,
            ChatFinishReasonDto.ToolCalls => ChatFinishReason.ToolCalls,
            ChatFinishReasonDto.ContentFilter => ChatFinishReason.ContentFilter,
            _ => null
        };
    }

    public ChatFinishReasonDto? MapToFinishReasonDto(ChatFinishReason? finishReason)
    {
        if (finishReason == null)
        {
            return null;
        }

        if (finishReason == ChatFinishReason.Stop)
        {
            return ChatFinishReasonDto.Stop;
        }

        if (finishReason == ChatFinishReason.Length)
        {
            return ChatFinishReasonDto.Length;
        }

        if (finishReason == ChatFinishReason.ToolCalls)
        {
            return ChatFinishReasonDto.ToolCalls;
        }

        if (finishReason == ChatFinishReason.ContentFilter)
        {
            return ChatFinishReasonDto.ContentFilter;
        }

        return ChatFinishReasonDto.Stop;
    }

    public ChatOptions? MapToChatOptions(ChatOptionsDto? options)
    {
        return options is null ? null : MapToChatOptionsNotNull(options);
    }

    private ChatOptions MapToChatOptionsNotNull(ChatOptionsDto optionsDto)
    {
        return new ChatOptions
        {
            ConversationId = optionsDto.ConversationId,
            Instructions = optionsDto.Instructions,
            Temperature = optionsDto.Temperature,
            MaxOutputTokens = optionsDto.MaxOutputTokens,
            TopP = optionsDto.TopP,
            TopK = optionsDto.TopK,
            FrequencyPenalty = optionsDto.FrequencyPenalty,
            PresencePenalty = optionsDto.PresencePenalty,
            Seed = optionsDto.Seed,
            ResponseFormat = MapToChatResponseFormat(optionsDto.ResponseFormat),
            ModelId = optionsDto.ModelId,
            StopSequences = optionsDto.StopSequences,
            AllowMultipleToolCalls = optionsDto.AllowMultipleToolCalls,
            ToolMode = MapToChatToolMode(optionsDto.ToolMode)
        };
    }

    private ChatResponseFormat? MapToChatResponseFormat(ChatResponseFormatDto? dto)
    {
        return dto switch
        {
            ChatResponseFormatTextDto => new ChatResponseFormatText(),
            ChatResponseFormatJsonDto json => new ChatResponseFormatJson(
                JsonDocument.Parse(json.Schema ?? "{}").RootElement,
                json.SchemaName,
                json.SchemaDescription
            ),
            _ => null
        };
    }

    private ChatToolMode? MapToChatToolMode(ChatToolModeDto? chatToolModeDto)
    {
        return chatToolModeDto switch
        {
            AutoChatToolModeDto => new AutoChatToolMode(),
            NoneChatToolModeDto => new NoneChatToolMode(),
            RequiredChatToolModeDto required => new RequiredChatToolMode(required.RequiredFunctionName),
            _ => null
        };
    }

    public ChatMessage MapToChatMessage(ChatMessageDto appModel)
    {
        return new ChatMessage(MapToChatRole(appModel.Role),
            [.. appModel.Contents.Select(aiContentMapper.MapToAiContent)]);
    }

    // Helper methods for reverse mapping specific types
    private ChatRole MapToChatRole(ChatRoleEnumDto appModel)
    {
        return appModel switch
        {
            ChatRoleEnumDto.System => ChatRole.System,
            ChatRoleEnumDto.User => ChatRole.User,
            ChatRoleEnumDto.Assistant => ChatRole.Assistant,
            ChatRoleEnumDto.Tool => ChatRole.Tool,
            _ => ChatRole.User
        };
    }

    private ChatRoleEnumDto MapToChatRoleDto(ChatRole role)
    {
        if (role == ChatRole.System)
        {
            return ChatRoleEnumDto.System;
        }

        if (role == ChatRole.User)
        {
            return ChatRoleEnumDto.User;
        }

        if (role == ChatRole.Assistant)
        {
            return ChatRoleEnumDto.Assistant;
        }

        if (role == ChatRole.Tool)
        {
            return ChatRoleEnumDto.Tool;
        }

        return ChatRoleEnumDto.User;
    }

    public ChatOptionsDto? MapToChatOptionsDto(ChatOptions? chatOptions)
    {
        if (chatOptions == null)
        {
            return null;
        }

        return new ChatOptionsDto(
            chatOptions.ConversationId,
            chatOptions.Instructions,
            chatOptions.Temperature,
            chatOptions.MaxOutputTokens,
            chatOptions.TopP,
            chatOptions.TopK,
            chatOptions.FrequencyPenalty,
            chatOptions.PresencePenalty,
            chatOptions.Seed,
            null,
            MapToChatResponseFormatDto(chatOptions.ResponseFormat),
            chatOptions.ModelId,
            chatOptions.StopSequences?.ToList(),
            chatOptions.AllowMultipleToolCalls,
            MapToChatToolModeDto(chatOptions.ToolMode),
            MapToAiToolDto(chatOptions.Tools));
    }

    private IDictionary<string, IList<AbstractAiToolDto>>? MapToAiToolDto(IList<AITool>? chatOptionsTools)
    {
        if (chatOptionsTools is null)
        {
            return null;
        }

        var toolsByServer =
            new Dictionary<string, IList<AbstractAiToolDto>> { ["default"] = [.. chatOptionsTools.Select(MapToAiToolDto)] };
        return toolsByServer;
    }

    private AbstractAiToolDto MapToAiToolDto(AITool tool)
    {
        return new AiToolDto(tool.Name, tool.Description);
    }

    private ChatToolModeDto? MapToChatToolModeDto(ChatToolMode? toolMode)
    {
        return toolMode switch
        {
            AutoChatToolMode => new AutoChatToolModeDto(),
            NoneChatToolMode => new NoneChatToolModeDto(),
            RequiredChatToolMode requiredToolMode => new RequiredChatToolModeDto(requiredToolMode.RequiredFunctionName),
            _ => null
        };
    }

    private ChatResponseFormatDto? MapToChatResponseFormatDto(ChatResponseFormat? chatOptionsResponseFormat)
    {
        if (chatOptionsResponseFormat == null)
        {
            return null;
        }

        if (chatOptionsResponseFormat is ChatResponseFormatText)
        {
            return new ChatResponseFormatTextDto();
        }

        if (chatOptionsResponseFormat is ChatResponseFormatJson chatResponseFormatJson)
        {
            return new ChatResponseFormatJsonDto(chatResponseFormatJson.Schema.ToString(),
                chatResponseFormatJson.SchemaName, chatResponseFormatJson.SchemaDescription);
        }

        return null;
    }

    public ChatMessageDto MapToChatMessageDto(ChatMessage chatMessage)
    {
        return new ChatMessageDto(
            MapToChatRoleDto(chatMessage.Role),
            [.. chatMessage.Contents.Select(aiContentMapper.MapToAiContentDto)]
        );
    }
}
