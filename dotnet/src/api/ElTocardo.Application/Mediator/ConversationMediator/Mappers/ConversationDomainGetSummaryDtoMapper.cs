using ElTocardo.Application.Dtos.Conversation;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using ElTocardo.Application.Mappers.Dtos;
using ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI;
using ElTocardo.Domain.Mediator.Common.Mappers;
using ElTocardo.Domain.Mediator.ConversationMediator.Entities;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mediator.ConversationMediator.Mappers;

public class ConversationDomainGetSummaryDtoMapper(
    IDomainEntityMapper<ChatOptions, ChatOptionsDto> chatOptionsMapper,
    IDomainEntityMapper<ChatFinishReason, ChatFinishReasonDto> chatFinishReasonMapper,
    IDomainEntityMapper<ChatMessage, ChatMessageDto> chatMessageMapper) : AbstractDomainGetDtoMapper<Conversation, string, string, ConversationSummaryDto>
{
    public override ConversationSummaryDto MapDomainToDto(Conversation conversation)
    {
        var rounds = conversation.Rounds;

        var latestResponse = rounds.LastOrDefault()?.Response;
        var chatOptionsDto = chatOptionsMapper.ToApplicationNullable(conversation.CurrentOptions);

        return new ConversationSummaryDto(
            conversation.Id,
            conversation.Title,
            conversation.Description,
            rounds.Count,
            conversation.CreatedAt,
            chatFinishReasonMapper.ToApplication(latestResponse?.FinishReason),
            chatOptionsDto,
            conversation.CurrentProvider);
    }

    private  IList<ChatMessageDto> GetMessagesDto(IList<ConversationRound> rounds)
    {
        var chatMessages = new List<ChatMessageDto>();

        foreach (var round in rounds)
        {
            chatMessages.Add(chatMessageMapper.ToApplication(round.InputMessage));
            if (round.Response != null)
            {
                chatMessages.AddRange(round.Response.Messages.Select(chatMessageMapper.ToApplication));
            }
        }
        return chatMessages;
    }
}
