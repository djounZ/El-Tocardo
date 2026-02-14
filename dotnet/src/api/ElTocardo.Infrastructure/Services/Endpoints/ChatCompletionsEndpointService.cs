using System.Runtime.CompilerServices;
using ElTocardo.Application.Dtos.ChatCompletion;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI;
using ElTocardo.Application.Services;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.Services.Endpoints;

public sealed class ChatCompletionsEndpointService(
    ILogger<ChatCompletionsEndpointService> logger,
    AiChatCompletionMapper aiChatCompletionMapper,
    ChatClientProvider clientProvider,
    AiToolsProviderService aiToolsProviderService) : AbstractChatCompletionsService(logger,  clientProvider, aiToolsProviderService), IChatCompletionsEndpointService
{
    public async Task<ChatResponseDto> ComputeChatCompletionsAsync(ChatRequestDto chatRequestDto, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Computing chat completions for Request: {@Request}", chatRequestDto);
        var request = await MapToAiChatClientRequest(chatRequestDto, cancellationToken);

        var chatClient = await ClientProvider.GetChatClientAsync(chatRequestDto.Provider, cancellationToken);
        var response = await chatClient.GetResponseAsync(request.Messages, request.Options, cancellationToken);
        Logger.LogTrace("Computing chat completions Response Received: {@Response}", response);
        return aiChatCompletionMapper.MapToChatResponseDto(response);
    }

    public async IAsyncEnumerable<ChatResponseUpdateDto> ComputeStreamingChatCompletionAsync(ChatRequestDto chatRequestDto, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Logger.LogInformation("Computing streaming chat completions for Request: {@Request}", chatRequestDto);
        var request = await MapToAiChatClientRequest(chatRequestDto, cancellationToken);
        var chatClient = await ClientProvider.GetChatClientAsync(chatRequestDto.Provider, cancellationToken);

        var chatCompletionStreamAsync = chatClient.GetStreamingResponseAsync(request.Messages, request.Options, cancellationToken);

        await foreach (var update in chatCompletionStreamAsync)
        {
            Logger.LogTrace("Computing chat completions streaming Update Received: {@Update}", update);
            yield return  aiChatCompletionMapper.MapToChatResponseUpdateDto(update);
        }
        Logger.LogInformation("Computing chat completions streaming ended");
    }

    private async Task<AiChatCompletionMapper.AiChatClientRequest> MapToAiChatClientRequest(ChatRequestDto chatRequestDto, CancellationToken cancellationToken)
    {
        var request = aiChatCompletionMapper.MapToAiChatClientRequest(chatRequestDto);
        await MapTools(chatRequestDto.Options, request.Options, cancellationToken);

        return request;
    }
}
