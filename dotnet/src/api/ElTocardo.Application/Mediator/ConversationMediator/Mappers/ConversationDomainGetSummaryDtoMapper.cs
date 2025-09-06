using ElTocardo.Application.Dtos.AI.ChatCompletion.Request;
using ElTocardo.Application.Dtos.Conversation;
using ElTocardo.Application.Mappers.Dtos.AI;
using ElTocardo.Domain.Mediator.Common.Mappers;
using ElTocardo.Domain.Mediator.ConversationMediator.Entities;

namespace ElTocardo.Application.Mediator.ConversationMediator.Mappers;

public class ConversationDomainGetSummaryDtoMapper(AiChatCompletionMapper aiChatCompletionMapper) : AbstractDomainGetDtoMapper<Conversation, string, string, ConversationSummaryDto>
{
    public override ConversationSummaryDto MapDomainToDto(Conversation conversation)
    {
        var rounds = conversation.Rounds;

        var latestResponse = rounds.LastOrDefault()?.Response;
        var chatOptionsDto = aiChatCompletionMapper.MapToChatOptionsDto(conversation.CurrentOptions);

        return new ConversationSummaryDto(
            conversation.Id,
            conversation.Title,
            conversation.Description,
            rounds.Count,
            conversation.CreatedAt,
            aiChatCompletionMapper.MapToFinishReasonDto(latestResponse?.FinishReason),
            chatOptionsDto,
            conversation.CurrentProvider);
    }

    private  IList<ChatMessageDto> GetMessagesDto(IList<ConversationRound> rounds)
    {
        var chatMessages = new List<ChatMessageDto>();

        foreach (var round in rounds)
        {
            chatMessages.Add(aiChatCompletionMapper.MapToChatMessageDto(round.InputMessage));
            if (round.Response != null)
            {
                chatMessages.AddRange(round.Response.Messages.Select(aiChatCompletionMapper.MapToChatMessageDto));
            }
        }
        return chatMessages;
    }
}
