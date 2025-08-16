using System.Runtime.CompilerServices;
using ElTocardo.Application.Dtos.AI.ChatCompletion.Request;
using ElTocardo.Application.Dtos.AI.ChatCompletion.Response;
using ElTocardo.Application.Dtos.Provider;
using ElTocardo.Application.Services;
using ElTocardo.Infrastructure.Mappers.Dtos.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.Services;

public sealed class ChatClientProvider(ILogger<ChatClientProvider> logger)
{

    // should be user specific, but for now we will not filter on user
    public IChatClient GetChatClient(AiProviderEnumDto? provider)
    {
        logger.LogInformation("Retrieving chat client for provider: {Provider}", provider);
        // This method should return an instance of IChatClient based on the provider.
        // The implementation details would depend on how you configure your AI services.
        throw new NotImplementedException("Implement the logic to retrieve the appropriate IChatClient based on the provider.");
    }
}

public sealed class ChatCompletionsService(ILogger<ChatCompletionsService> logger, AiChatCompletionMapper mapper, ChatClientProvider clientProvider) : IChatCompletionsService
{
    public async Task<ChatResponseDto> ComputeChatCompletionsAsync(ChatRequestDto chatRequestDto, CancellationToken cancellationToken)
    {
        logger.LogInformation("Computing chat completions for Request: {@Request}", chatRequestDto);
        var request = mapper.MapToAiChatClientRequest(chatRequestDto);
        var chatClient = clientProvider.GetChatClient(chatRequestDto.Provider);
        var response = await chatClient.GetResponseAsync(request.Messages, request.Options, cancellationToken);
        logger.LogTrace("Computing chat completions Response Received: {@Response}", response);
        return mapper.MapToChatResponseDto(response);
    }

    public async IAsyncEnumerable<ChatResponseUpdateDto> ComputeStreamingChatCompletionAsync(ChatRequestDto chatRequestDto, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        logger.LogInformation("Computing streaming chat completions for Request: {@Request}", chatRequestDto);
        var request = mapper.MapToAiChatClientRequest(chatRequestDto);
        var chatClient = clientProvider.GetChatClient(chatRequestDto.Provider);

        var chatCompletionStreamAsync = chatClient.GetStreamingResponseAsync(request.Messages, request.Options, cancellationToken);

        await foreach (var update in chatCompletionStreamAsync)
        {
            logger.LogTrace("Computing chat completions streaming Update Received: {@Update}", update);
            yield return mapper.MapToChatResponseUpdateDto(update);
        }
        logger.LogInformation("Computing chat completions streaming ended");
    }
}
