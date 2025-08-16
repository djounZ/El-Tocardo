using AI.GithubCopilot.Infrastructure.Mappers.Dtos;
using AI.GithubCopilot.Infrastructure.Services;
using Microsoft.Extensions.AI;

namespace AI.GithubCopilot.Domain.Services;

public sealed class GithubCopilotChatClient(GithubCopilotChatCompletion githubCopilotChatCompletion, ChatCompletionMapper mapper) : IChatClient
{
    public void Dispose()
    {
       // no need to dispose anything;
    }

    public async Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null,
        CancellationToken cancellationToken = new())
    {
        var chatCompletionRequest =  mapper.MapToChatCompletionRequestDto(new ChatCompletionMapper.ChatCompletionRequest(messages, options));
        var chatCompletionResponse = await githubCopilotChatCompletion.GetChatCompletionAsync(chatCompletionRequest, cancellationToken);
        return  mapper.MapToChatResponse(chatCompletionResponse);
    }

    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null,
        CancellationToken cancellationToken = new())
    {
        var chatCompletionRequest =  mapper.MapToChatCompletionRequestDto(new ChatCompletionMapper.ChatCompletionRequest(messages, options));
        var chatCompletionStreamAsync = githubCopilotChatCompletion.GetChatCompletionStreamAsync(chatCompletionRequest, cancellationToken);
        return  mapper.MapToChatResponseUpdates(chatCompletionStreamAsync, cancellationToken);
    }

    public object? GetService(Type serviceType, object? serviceKey)
    {
        return
            serviceKey is not null ? null :
            serviceType.IsInstanceOfType(this) ? this :
            null;
    }

}
