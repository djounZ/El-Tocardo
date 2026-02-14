using ElTocardo.Application.Dtos.ChatCompletion;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;

namespace ElTocardo.Application.Services;

public interface IChatCompletionsEndpointService
{
    public Task<ChatResponseDto> ComputeChatCompletionsAsync(ChatRequestDto chatRequestDto, CancellationToken cancellationToken);
    public IAsyncEnumerable<ChatResponseUpdateDto>  ComputeStreamingChatCompletionAsync(ChatRequestDto chatRequestDto, CancellationToken cancellationToken);
}
