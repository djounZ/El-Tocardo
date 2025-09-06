using ElTocardo.Application.Dtos.AI.ChatCompletion.Request;
using ElTocardo.Application.Dtos.AI.ChatCompletion.Response;

namespace ElTocardo.Application.Services;

public interface IChatCompletionsEndpointService
{
    public Task<ChatResponseDto> ComputeChatCompletionsAsync(ChatRequestDto chatRequestDto, CancellationToken cancellationToken);
    public IAsyncEnumerable<ChatResponseUpdateDto>  ComputeStreamingChatCompletionAsync(ChatRequestDto chatRequestDto, CancellationToken cancellationToken);
}
