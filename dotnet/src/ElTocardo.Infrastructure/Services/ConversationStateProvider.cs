using ElTocardo.Application.Dtos.AI.ChatCompletion.Request;
using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Dtos.Conversation;
using Microsoft.Extensions.AI;

namespace ElTocardo.Infrastructure.Services;


public record ConversationState(AiProviderEnumDto? PreviousProvider, ChatOptionsDto? PreviousOptions, IList<ChatMessageDto> PreviousMessages);
public sealed class ConversationStateProvider
{
    public async Task<string> StartConversationAsync(StartConversationRequestDto startConversationRequestDto,
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        throw new System.NotImplementedException();
    }



    public async Task<ConversationState> GetAndUpdateConversationStateAsync(ContinueConversationDto continueConversationDto, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }


    public async Task UpdateConversationStateAsync(string conversationId, ChatResponse response, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }




    public async Task UpdateConversationStateAsync(string conversationId, IEnumerable<ChatResponseUpdate> updates, CancellationToken cancellationToken)
    {
        await UpdateConversationStateAsync(conversationId, updates.ToChatResponse(), cancellationToken);
    }
}
