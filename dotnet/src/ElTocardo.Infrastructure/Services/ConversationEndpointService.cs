using System.Runtime.CompilerServices;
using ElTocardo.Application.Dtos.Conversation;
using ElTocardo.Application.Services;
using ElTocardo.Infrastructure.Mappers.Dtos.AI;
using ElTocardo.Infrastructure.Mappers.Dtos.Conversation;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.Services;

public sealed class ConversationEndpointService(ILogger<ConversationEndpointService> logger,
    AiChatCompletionMapper aiChatCompletionMapper,
    ChatClientProvider clientProvider,
    AiToolsProviderService aiToolsProviderService,
    ConversationStateProvider conversationStateProvider,
    ConversationDtoChatDtoMapper conversationDtoChatDtoMapper) : AbstractChatCompletionsService(logger, aiChatCompletionMapper, clientProvider, aiToolsProviderService),IConversationEndpointService
{
    public async Task<ConversationResponseDto> StartConversationAsync(StartConversationRequestDto startConversationRequestDto,
        CancellationToken cancellationToken)
    {
        var conversationId = await conversationStateProvider.StartConversationAsync(startConversationRequestDto, cancellationToken);
        logger.LogInformation("Starting conversation with Request: {@Request} and Conversation Id: {ConversationId}", startConversationRequestDto, conversationId);

        var request = await MapToAiChatClientRequest(startConversationRequestDto, cancellationToken);

        var chatClient = await ClientProvider.GetChatClientAsync(startConversationRequestDto.InitialProvider, cancellationToken);
        var response = await chatClient.GetResponseAsync(request.Messages, request.Options, cancellationToken);
        var updateTask = conversationStateProvider.UpdateConversationStateAsync(conversationId, response, cancellationToken);
        var conversationResponseDto = conversationDtoChatDtoMapper.MapToConversationResponseDto(conversationId, response);
        logger.LogTrace("First Response Completed for Conversation Id : {ConversationId}", conversationId);
        await updateTask;
        return conversationResponseDto;
    }

    public async IAsyncEnumerable<ConversationUpdateResponseDto> StartStreamingConversationAsync(StartConversationRequestDto startConversationRequestDto,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var conversationId = await conversationStateProvider.StartConversationAsync(startConversationRequestDto, cancellationToken);
        logger.LogInformation("Starting Streaming conversation with Request: {@Request} and Conversation Id: {ConversationId}", startConversationRequestDto, conversationId);

        var request = await MapToAiChatClientRequest(startConversationRequestDto, cancellationToken);
        var chatClient = await ClientProvider.GetChatClientAsync(startConversationRequestDto.InitialProvider, cancellationToken);

        var chatCompletionStreamAsync = chatClient.GetStreamingResponseAsync(request.Messages, request.Options, cancellationToken);


        var chatResponseUpdates = new List<ChatResponseUpdate>();
        await foreach (var update in chatCompletionStreamAsync)
        {
            Logger.LogTrace("Computing chat completions streaming Update Received: {@Update}", update);
            chatResponseUpdates.Add(update);
            yield return   conversationDtoChatDtoMapper.MapToConversationUpdateResponseDto(conversationId, update);
        }
        logger.LogTrace("First Streaming Response Completed for Conversation Id : {ConversationId}", conversationId);
        await conversationStateProvider.UpdateConversationStateAsync(conversationId, chatResponseUpdates, cancellationToken);
    }

    public async Task<ConversationResponseDto> ContinueConversationAsync(ContinueConversationDto continueConversationDto, CancellationToken cancellationToken)
    {
        var conversationId = continueConversationDto.ConversationId;
        logger.LogInformation("Continue conversation with Request: {@Request} for Conversation Id: {ConversationId}", continueConversationDto, conversationId);
        var conversationState = await conversationStateProvider.GetAndUpdateConversationStateAsync(continueConversationDto, cancellationToken);
        var provider = continueConversationDto.Provider ?? conversationState.PreviousProvider;
        var request = await MapToAiChatClientRequest(continueConversationDto, conversationState, cancellationToken);

        var chatClient = await ClientProvider.GetChatClientAsync(provider, cancellationToken);
        var response = await chatClient.GetResponseAsync(request.Messages, request.Options, cancellationToken);
        var updateTask = conversationStateProvider.UpdateConversationStateAsync(conversationId, response, cancellationToken);
        var conversationResponseDto = conversationDtoChatDtoMapper.MapToConversationResponseDto(conversationId, response);
        logger.LogTrace("Response Received for Conversation Id : {ConversationId}", conversationId);
        await updateTask;
        return conversationResponseDto;
    }

    public async IAsyncEnumerable<ConversationUpdateResponseDto> ContinueStreamingConversationAsync(ContinueConversationDto continueConversationDto,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var conversationId = continueConversationDto.ConversationId;
        logger.LogInformation("Continue conversation with Request: {@Request} for Conversation Id: {ConversationId}", continueConversationDto, conversationId);

        var conversationState = await conversationStateProvider.GetAndUpdateConversationStateAsync(continueConversationDto, cancellationToken);
        var provider = continueConversationDto.Provider ?? conversationState.PreviousProvider;

        var request = await MapToAiChatClientRequest(continueConversationDto, conversationState, cancellationToken);
        var chatClient = await ClientProvider.GetChatClientAsync(provider, cancellationToken);

        var chatCompletionStreamAsync = chatClient.GetStreamingResponseAsync(request.Messages, request.Options, cancellationToken);

        var chatResponseUpdates = new List<ChatResponseUpdate>();
        await foreach (var update in chatCompletionStreamAsync)
        {
            Logger.LogTrace("Computing chat completions streaming Update Received: {@Update}", update);
            chatResponseUpdates.Add(update);
            yield return   conversationDtoChatDtoMapper.MapToConversationUpdateResponseDto(conversationId, update);
        }

        await conversationStateProvider.UpdateConversationStateAsync(conversationId, chatResponseUpdates, cancellationToken);

        logger.LogTrace("Response Completed for Conversation Id : {ConversationId}", conversationId);
    }

    private async Task<AiChatCompletionMapper.AiChatClientRequest> MapToAiChatClientRequest(ContinueConversationDto continueConversationDto,ConversationState conversationState, CancellationToken cancellationToken)
    {
        var chatRequestDto=conversationDtoChatDtoMapper.MapToChatRequestDto(continueConversationDto, conversationState.PreviousMessages,
            conversationState.PreviousProvider, conversationState.PreviousOptions);
        var request = AiChatCompletionMapper.MapToAiChatClientRequest(chatRequestDto);
        await MapTools(chatRequestDto.Options, request, cancellationToken);
        return request;
    }



    private async Task<AiChatCompletionMapper.AiChatClientRequest> MapToAiChatClientRequest(StartConversationRequestDto startConversationRequestDto, CancellationToken cancellationToken)
    {
        var chatRequestDto = conversationDtoChatDtoMapper.MapToChatRequestDto(startConversationRequestDto);
        var request = AiChatCompletionMapper.MapToAiChatClientRequest(chatRequestDto);
        await MapTools(chatRequestDto.Options, request, cancellationToken);

        return request;
    }

}
