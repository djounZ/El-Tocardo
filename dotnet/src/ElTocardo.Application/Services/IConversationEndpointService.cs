using ElTocardo.Application.Dtos.Conversation;

namespace ElTocardo.Application.Services;

public interface IConversationEndpointService
{
    public Task<ConversationResponseDto> StartConversationAsync(StartConversationRequestDto startConversationRequestDto, CancellationToken cancellationToken);
    public IAsyncEnumerable<ConversationUpdateResponseDto>  StartStreamingConversationAsync(StartConversationRequestDto startConversationRequestDto, CancellationToken cancellationToken);
    public Task<ConversationResponseDto> ContinueConversationAsync(ContinueConversationDto continueConversationDto, CancellationToken cancellationToken);
    public IAsyncEnumerable<ConversationUpdateResponseDto>  ContinueStreamingConversationAsync(ContinueConversationDto continueConversationDto, CancellationToken cancellationToken);
}
