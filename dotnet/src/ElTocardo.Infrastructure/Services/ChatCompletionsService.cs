using System.Runtime.CompilerServices;
using ElTocardo.Application.Dtos.AI.ChatCompletion.Request;
using ElTocardo.Application.Dtos.AI.ChatCompletion.Response;
using ElTocardo.Application.Services;
using ElTocardo.Infrastructure.Mappers.Dtos.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.Services;

public sealed class ChatCompletionsService(ILogger<ChatCompletionsService> logger, AiChatCompletionMapper mapper, ChatClientProvider clientProvider, AiToolsProviderService aiToolsProviderService) : IChatCompletionsService
{
    public async Task<ChatResponseDto> ComputeChatCompletionsAsync(ChatRequestDto chatRequestDto, CancellationToken cancellationToken)
    {
        logger.LogInformation("Computing chat completions for Request: {@Request}", chatRequestDto);
        var request = await MapToAiChatClientRequest(chatRequestDto, cancellationToken);

        var chatClient = await clientProvider.GetChatClientAsync(chatRequestDto.Provider, cancellationToken);
        var response = await chatClient.GetResponseAsync(request.Messages, request.Options, cancellationToken);
        logger.LogTrace("Computing chat completions Response Received: {@Response}", response);
        return mapper.MapToChatResponseDto(response);
    }

    public async IAsyncEnumerable<ChatResponseUpdateDto> ComputeStreamingChatCompletionAsync(ChatRequestDto chatRequestDto, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        logger.LogInformation("Computing streaming chat completions for Request: {@Request}", chatRequestDto);
        var request = await MapToAiChatClientRequest(chatRequestDto, cancellationToken);
        var chatClient = await clientProvider.GetChatClientAsync(chatRequestDto.Provider, cancellationToken);

        var chatCompletionStreamAsync = chatClient.GetStreamingResponseAsync(request.Messages, request.Options, cancellationToken);

        await foreach (var update in chatCompletionStreamAsync)
        {
            logger.LogTrace("Computing chat completions streaming Update Received: {@Update}", update);
            yield return mapper.MapToChatResponseUpdateDto(update);
        }
        logger.LogInformation("Computing chat completions streaming ended");
    }

    private async Task<AiChatCompletionMapper.AiChatClientRequest> MapToAiChatClientRequest(ChatRequestDto chatRequestDto, CancellationToken cancellationToken)
    {
        var request = mapper.MapToAiChatClientRequest(chatRequestDto);
        if (chatRequestDto.Options?.Tools != null)
        {
            request.Options!.Tools = await aiToolsProviderService.GetAiToolsAsync(chatRequestDto.Options.Tools, cancellationToken);
        }

        return request;
    }
}
