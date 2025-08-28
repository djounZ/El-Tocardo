using ElTocardo.Application.Dtos.AI.ChatCompletion.Request;
using ElTocardo.Application.Dtos.Conversation;
using ElTocardo.Domain.Models;

namespace ElTocardo.Application.Services;

public interface IConversationEndpointService
{
    public Task<ConversationResponseDto> StartConversationAsync(StartConversationRequestDto startConversationRequestDto, CancellationToken cancellationToken);
    public IAsyncEnumerable<ConversationUpdateResponseDto>  StartStreamingConversationAsync(StartConversationRequestDto startConversationRequestDto, CancellationToken cancellationToken);
    public Task<ConversationResponseDto> ContinueConversationAsync(ContinueConversationDto continueConversationDto, CancellationToken cancellationToken);
    public IAsyncEnumerable<ConversationUpdateResponseDto>  ContinueStreamingConversationAsync(ContinueConversationDto continueConversationDto, CancellationToken cancellationToken);

    public Task<Result<ConversationDto>> GetConversation(string conversationId, CancellationToken cancellationToken);
}
