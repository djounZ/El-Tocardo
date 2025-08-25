using ElTocardo.Application.Dtos.Conversation;
using ElTocardo.Application.Mappers.Dtos.AI;
using ElTocardo.Application.Mediator.Common.Mappers;
using ElTocardo.Domain.Mediator.ConversationMediator.Entities;

namespace ElTocardo.Application.Mediator.ConversationMediator.Mappers;

public class ConversationDomainGetDtoMapper(AiChatCompletionMapper aiChatCompletionMapper) : AbstractDomainGetDtoMapper<Conversation, string, string, ConversationResponseDto>
{
    public override ConversationResponseDto MapDomainToDto(Conversation conversation)
    {
        // Get the latest response messages from the last round
        var latestMessages = conversation.Rounds.LastOrDefault()?.Response?.Messages ?? new List<Microsoft.Extensions.AI.ChatMessage>();
        var latestResponse = conversation.Rounds.LastOrDefault()?.Response;

        var messagesDto = latestMessages.Select(aiChatCompletionMapper.MapToChatMessageDto).ToList();

        return new ConversationResponseDto(
            conversation.Id.ToString(),
            messagesDto,
            latestResponse?.ModelId,
            latestResponse?.CreatedAt,
            latestResponse?.FinishReason != null ? aiChatCompletionMapper.MapToFinishReasonDto(latestResponse.FinishReason) : null);
    }
}
