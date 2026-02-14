using ElTocardo.Application.Dtos.Microsoft.Extensions.AI;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.ChatCompletion;

public class ChatResponseMapper(
    IDomainEntityMapper<ChatMessage, ChatMessageDto> chatMessageMapper,
    IDomainEntityMapper<ChatFinishReason, ChatFinishReasonDto> chatFinishReasonMapper,
    IDomainEntityMapper<UsageDetails, UsageDetailsDto> usageDetailsMapper
) : IDomainEntityMapper<ChatResponse, ChatResponseDto>
{

    public ChatResponseDto ToApplication(ChatResponse domainItem)
    {
        var messages = domainItem.Messages.Select(chatMessageMapper.ToApplication).ToList();
        var finishReason = chatFinishReasonMapper.ToApplication(domainItem.FinishReason);
        var usage = usageDetailsMapper.ToApplicationNullable(domainItem.Usage);
        var responseId = domainItem.ResponseId;
        var conversationId = domainItem.ConversationId;
        var modelId = domainItem.ModelId;
        var createdAt = domainItem.CreatedAt;

        return new ChatResponseDto(
            messages,
            responseId,
            conversationId,
            modelId,
            createdAt,
            finishReason,
            usage);
    }
    public ChatResponse ToDomain(ChatResponseDto applicationItem)
    {
        var messages = applicationItem.Messages.Select(chatMessageMapper.ToDomain).ToList();
        var finishReason = chatFinishReasonMapper.ToDomain(applicationItem.FinishReason);
        var usage = usageDetailsMapper.ToDomainNullable(applicationItem.Usage);
        var responseId = applicationItem.ResponseId;
        var conversationId = applicationItem.ConversationId;
        var modelId = applicationItem.ModelId;
        var createdAt = applicationItem.CreatedAt;

        return new ChatResponse
        {
            Messages = messages,
            ResponseId = responseId,
            ConversationId = conversationId,
            ModelId = modelId,
            CreatedAt = createdAt,
            FinishReason = finishReason,
            Usage = usage
        };
    }
}