using System.Runtime.CompilerServices;
using ElTocardo.Application.Dtos.AI.ChatCompletion.Request;
using ElTocardo.Application.Dtos.AI.ChatCompletion.Response;
using ElTocardo.Application.Services;
using ElTocardo.Infrastructure.Mappers.Dtos.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.Services.Endpoints;

public sealed class ChatCompletionsEndpointService(
    ILogger<ChatCompletionsEndpointService> logger,
    AiChatCompletionMapper aiChatCompletionMapper,
    ChatClientProvider clientProvider,
    AiToolsProviderService aiToolsProviderService) : AbstractChatCompletionsService(logger, aiChatCompletionMapper, clientProvider, aiToolsProviderService), IChatCompletionsEndpointService
{
    public async Task<ChatResponseDto> ComputeChatCompletionsAsync(ChatRequestDto chatRequestDto, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Computing chat completions for Request: {@Request}", chatRequestDto);
        var request = await MapToAiChatClientRequest(chatRequestDto, cancellationToken);

        var chatClient = await ClientProvider.GetChatClientAsync(chatRequestDto.Provider, cancellationToken);
        var response = await chatClient.GetResponseAsync(request.Messages, request.Options, cancellationToken);
        Logger.LogTrace("Computing chat completions Response Received: {@Response}", response);
        return AiChatCompletionMapper.MapToChatResponseDto(response);
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
            yield return  AiChatCompletionMapper.MapToChatResponseUpdateDto(update);
        }
        Logger.LogInformation("Computing chat completions streaming ended");
    }

    private async Task<AiChatCompletionMapper.AiChatClientRequest> MapToAiChatClientRequest(ChatRequestDto chatRequestDto, CancellationToken cancellationToken)
    {
        var request = AiChatCompletionMapper.MapToAiChatClientRequest(chatRequestDto);
        await MapTools(chatRequestDto.Options, request, cancellationToken);

        return request;
    }
}
