using System.Runtime.CompilerServices;
using ElTocardo.Application.Dtos.ChatCompletion;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using ElTocardo.Application.Mappers.Dtos;
using ElTocardo.Application.Services;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.Services.Endpoints;

public sealed class ChatCompletionsEndpointService(
    ILogger<ChatCompletionsEndpointService> logger,
    IDomainEntityMapper<ChatOptions, ChatOptionsDto> chatOptionsMapper,
    IDomainEntityMapper<ChatMessage, ChatMessageDto> chatMessageMapper,
    IDomainEntityMapper<ChatResponse, ChatResponseDto> chatResponseMapper,
    IDomainEntityMapper<ChatResponseUpdate, ChatResponseUpdateDto> chatResponseUpdateMapper,
    ChatClientProvider clientProvider,
    AiToolsProviderService aiToolsProviderService) : AbstractChatCompletionsService(logger,  clientProvider, aiToolsProviderService), IChatCompletionsEndpointService
{
    public async Task<ChatResponseDto> ComputeChatCompletionsAsync(ChatRequestDto chatRequestDto, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Computing chat completions for Request: {@Request}", chatRequestDto);
        var chatOptions = chatOptionsMapper.ToDomainNullable(chatRequestDto.Options);
        ChatMessage[] chatMessages=  [.. chatRequestDto.Messages.Select(chatMessageMapper.ToDomain)];

        await MapTools(chatRequestDto.Options, chatOptions, cancellationToken);

        var chatClient = await ClientProvider.GetChatClientAsync(chatRequestDto.Provider, cancellationToken);
        var response = await chatClient.GetResponseAsync(chatMessages, chatOptions, cancellationToken);
        Logger.LogTrace("Computing chat completions Response Received: {@Response}", response);
        return chatResponseMapper.ToApplication(response);
    }

    public async IAsyncEnumerable<ChatResponseUpdateDto> ComputeStreamingChatCompletionAsync(ChatRequestDto chatRequestDto, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Logger.LogInformation("Computing streaming chat completions for Request: {@Request}", chatRequestDto);

        var chatOptions = chatOptionsMapper.ToDomainNullable(chatRequestDto.Options);
        ChatMessage[] chatMessages=  [.. chatRequestDto.Messages.Select(chatMessageMapper.ToDomain)];
        await MapTools(chatRequestDto.Options, chatOptions, cancellationToken);

        var chatClient = await ClientProvider.GetChatClientAsync(chatRequestDto.Provider, cancellationToken);

        var chatCompletionStreamAsync = chatClient.GetStreamingResponseAsync(chatMessages, chatOptions, cancellationToken);

        await foreach (var update in chatCompletionStreamAsync)
        {
            Logger.LogTrace("Computing chat completions streaming Update Received: {@Update}", update);
            yield return  chatResponseUpdateMapper.ToApplication(update);
        }
        Logger.LogInformation("Computing chat completions streaming ended");
    }
}
