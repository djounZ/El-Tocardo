using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.ChatCompletion;

public class ChatResponseUpdateMapper(
    IDomainEntityMapper<ChatFinishReason, ChatFinishReasonDto> chatFinishReasonMapper,
    IDomainEntityMapper<ChatRole, ChatRoleEnumDto> chatRoleMapper,
    IDomainEntityMapper<AIContent, AiContentDto> aiContentMapper
) : IDomainEntityMapper<ChatResponseUpdate, ChatResponseUpdateDto>
{

    public ChatResponseUpdateDto ToApplication(ChatResponseUpdate domainItem)
    {


        var authorName = domainItem.AuthorName;
        var contents = domainItem.Contents.Select(aiContentMapper.ToApplication).ToList();
        var createdAt = domainItem.CreatedAt;
        var conversationId = domainItem.ConversationId;
        var finishReason = chatFinishReasonMapper.ToApplication(domainItem.FinishReason);
        var messageId = domainItem.MessageId;
        var modelId = domainItem.ModelId;
        var responseId = domainItem.ResponseId;
        var role = chatRoleMapper.ToApplication(domainItem.Role);

        var result = new ChatResponseUpdateDto(
            authorName,
            role,
            contents,
            responseId,
            messageId,
            conversationId,
            createdAt,
            finishReason,
            modelId
        );

        return result;
    }
    public ChatResponseUpdate ToDomain(ChatResponseUpdateDto applicationItem)
    {
        var authorName = applicationItem.AuthorName;
        var contents = applicationItem.Contents.Select(aiContentMapper.ToDomain).ToList();
        var createdAt = applicationItem.CreatedAt;
        var conversationId = applicationItem.ConversationId;
        var finishReason = chatFinishReasonMapper.ToDomain(applicationItem.FinishReason);
        var messageId = applicationItem.MessageId;
        var modelId = applicationItem.ModelId;
        var responseId = applicationItem.ResponseId;
        var role = chatRoleMapper.ToDomain(applicationItem.Role);

        var result = new ChatResponseUpdate
        {
            AuthorName = authorName,
            Contents = contents,
            CreatedAt = createdAt,
            ConversationId = conversationId,
            FinishReason = finishReason,
            MessageId = messageId,
            ModelId = modelId,
            ResponseId = responseId,
            Role = role,
        };

        return result;
    }
}