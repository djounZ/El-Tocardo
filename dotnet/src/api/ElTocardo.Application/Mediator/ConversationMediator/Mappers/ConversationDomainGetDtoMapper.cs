using ElTocardo.Application.Dtos.Conversation;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI;
using ElTocardo.Domain.Mediator.Common.Mappers;
using ElTocardo.Domain.Mediator.ConversationMediator.Entities;

namespace ElTocardo.Application.Mediator.ConversationMediator.Mappers;

public class ConversationDomainGetDtoMapper(AiChatCompletionMapper aiChatCompletionMapper) : AbstractDomainGetDtoMapper<Conversation, string, string, ConversationDto>
{
    public override ConversationDto MapDomainToDto(Conversation conversation)
    {
        var rounds = conversation.Rounds;

        // Get the latest response messages from the last round
        var messagesDto = GetMessagesDto(rounds);
        var latestResponse = rounds.LastOrDefault()?.Response;
        var chatOptionsDto = aiChatCompletionMapper.MapToChatOptionsDto(conversation.CurrentOptions);

        return new ConversationDto(
            conversation.Id,
            conversation.Title,
            conversation.Description,
            messagesDto,
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
