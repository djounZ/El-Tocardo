using System.Runtime.CompilerServices;
using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Dtos.Conversation;
using ElTocardo.Application.Mappers.Dtos.Conversation;
using ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI;
using ElTocardo.Application.Mediator.ConversationMediator.Commands;
using ElTocardo.Application.Mediator.ConversationMediator.Queries;
using ElTocardo.Application.Services;
using ElTocardo.Domain.Mediator.Common.Interfaces;
using ElTocardo.Domain.Mediator.ConversationMediator.Entities;
using ElTocardo.Domain.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.Services;

public sealed class ConversationEndpointService(ILogger<ConversationEndpointService> logger,
    IQueryHandler<GetAllConversationsQuery, ConversationSummaryDto[]> conversationSummariesHandler,
    ICommandHandler<CreateConversationCommand, string> createConversationCommandHandler,
    ICommandHandler<UpdateConversationUpdateRoundCommand,Conversation> updateRoundCommandHandler,
    ICommandHandler<UpdateConversationAddNewRoundCommand,Conversation> addNewRoundCommandHandler,
    IQueryHandler<GetConversationByIdQuery, ConversationDto> getConversationByIdQueryHandler,
    ChatClientProvider clientProvider,
    AiToolsProviderService aiToolsProviderService,
    ConversationDtoChatDtoMapper conversationDtoChatDtoMapper,
    ICommandHandler<DeleteConversationCommand> deleteCommandHandler) : AbstractChatCompletionsService(logger,  clientProvider, aiToolsProviderService),IConversationEndpointService
{
    public async Task<ConversationResponseDto> StartConversationAsync(StartConversationRequestDto startConversationRequestDto,
        CancellationToken cancellationToken)
    {

        var (chatMessages, chatOptions, conversationId, chatClient) = await GetChatRequestContextAsync(startConversationRequestDto, cancellationToken);

        logger.LogInformation("Starting conversation with Request: {@Request} and Conversation Id: {ConversationId}", startConversationRequestDto, conversationId);
        var response = await chatClient.GetResponseAsync(chatMessages, chatOptions, cancellationToken);

        var conversationResponseDto = await UpdateConversationRound(conversationId, response, cancellationToken);

        logger.LogTrace("First Response Completed for Conversation Id : {ConversationId}", conversationId);
        return conversationResponseDto;
    }

    public async IAsyncEnumerable<ConversationUpdateResponseDto> StartStreamingConversationAsync(StartConversationRequestDto startConversationRequestDto,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {

        var (chatMessages, chatOptions, conversationId, chatClient) = await GetChatRequestContextAsync(startConversationRequestDto, cancellationToken);

        logger.LogInformation("Starting Streaming conversation with Request: {@Request} and Conversation Id: {ConversationId}", startConversationRequestDto, conversationId);
        var chatCompletionStreamAsync = chatClient.GetStreamingResponseAsync(chatMessages, chatOptions, cancellationToken);


        await foreach (var updateResponseDto in HandleStreamingResponseAsync(chatCompletionStreamAsync, conversationId, cancellationToken))
        {
            yield return updateResponseDto;
        }
        logger.LogTrace("First Streaming Response Completed for Conversation Id : {ConversationId}", conversationId);
    }

    public async Task<ConversationResponseDto> ContinueConversationAsync(ContinueConversationDto continueConversationDto, CancellationToken cancellationToken)
    {

        var (chatMessages, chatOptions, conversationId, chatClient) = await GetChatRequestContextAsync(continueConversationDto, cancellationToken);

        logger.LogInformation("Continue conversation with Request: {@Request} for Conversation Id: {ConversationId}", continueConversationDto, conversationId);
        var response = await chatClient.GetResponseAsync(chatMessages, chatOptions, cancellationToken);
        var conversationResponseDto = await UpdateConversationRound(conversationId, response, cancellationToken);

        logger.LogTrace("Response Received for Conversation Id : {ConversationId}", conversationId);
        return conversationResponseDto;
    }

    public async IAsyncEnumerable<ConversationUpdateResponseDto> ContinueStreamingConversationAsync(ContinueConversationDto continueConversationDto,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var (chatMessages, chatOptions, conversationId, chatClient) = await GetChatRequestContextAsync(continueConversationDto, cancellationToken);

        logger.LogInformation("Continue streaming conversation with Request: {@Request} for Conversation Id: {ConversationId}", continueConversationDto, conversationId);
        var chatCompletionStreamAsync = chatClient.GetStreamingResponseAsync(chatMessages, chatOptions, cancellationToken);

        await foreach (var updateResponseDto in HandleStreamingResponseAsync(chatCompletionStreamAsync, conversationId, cancellationToken))
        {
            yield return updateResponseDto;
        }

        logger.LogTrace("Streaming Response Completed for Conversation Id : {ConversationId}", conversationId);
    }

    public async Task<Result<ConversationDto>> GetConversation(string conversationId, CancellationToken cancellationToken)
    {
        return await getConversationByIdQueryHandler.HandleAsync(new GetConversationByIdQuery(conversationId), cancellationToken);
    }

    public async Task<Result<ConversationSummaryDto[]>> GetConversations(CancellationToken cancellationToken)
    {
       return await conversationSummariesHandler.HandleAsync(GetAllConversationsQuery.Instance, cancellationToken);
    }

    public async Task<VoidResult> DeleteConversationAsync(string conversationId, CancellationToken cancellationToken)
    {
       return await deleteCommandHandler.HandleAsync(new DeleteConversationCommand(conversationId), cancellationToken);
    }

    private record ChatRequestContext(IEnumerable<ChatMessage> ChatMessages, ChatOptions? ChatOptions, string ConversationId, IChatClient ChatClient);

    private async Task<ChatRequestContext> GetChatRequestContextAsync(StartConversationRequestDto startConversationRequestDto,
        CancellationToken cancellationToken)
    {


        var (chatMessage, options) = conversationDtoChatDtoMapper.MapToChatMessageAndOptions(startConversationRequestDto);
        await MapTools(startConversationRequestDto.Options, options, cancellationToken);

        var createConversationCommand = new CreateConversationCommand(startConversationRequestDto.Title, startConversationRequestDto.Description, chatMessage, options, startConversationRequestDto.Provider?.ToString());
        var conversationCreation = await createConversationCommandHandler.HandleAsync(createConversationCommand, cancellationToken);

        var conversationId = conversationCreation.ReadValue();

        var chatClient = await ClientProvider.GetChatClientAsync(startConversationRequestDto.Provider, cancellationToken);

        return new ChatRequestContext([chatMessage], options, conversationId, chatClient);
    }

    private async Task<ChatRequestContext> GetChatRequestContextAsync(ContinueConversationDto continueConversationDto,
        CancellationToken cancellationToken)
    {

        var (chatMessage, options) = conversationDtoChatDtoMapper.MapToChatMessageAndOptions(continueConversationDto);
        await MapTools(continueConversationDto.Options, options, cancellationToken);
        var newRoundCommand = new UpdateConversationAddNewRoundCommand(continueConversationDto.ConversationId, chatMessage, options, continueConversationDto.Provider?.ToString());
        var newRoundResult = await addNewRoundCommandHandler.HandleAsync(newRoundCommand, cancellationToken);

        var conversation = newRoundResult.ReadValue();

        var request = new AiChatClientRequest(conversation.Rounds.SelectMany(m=> m.GetAllMessages()), conversation.CurrentOptions);

        var chatClient = await ClientProvider.GetChatClientAsync(Enum.Parse<AiProviderEnumDto>(conversation.CurrentProvider), cancellationToken);

        return new ChatRequestContext(request.Messages, request.Options, continueConversationDto.ConversationId, chatClient);
    }

    private async Task<ConversationResponseDto> UpdateConversationRound(string conversationId,
        ChatResponse response, CancellationToken cancellationToken)
    {
        var updateConversationWithChatResponseCommand = new UpdateConversationUpdateRoundCommand(conversationId, response);
        var insertTask = updateRoundCommandHandler.HandleAsync(updateConversationWithChatResponseCommand, cancellationToken);

        var conversationResponseDto = conversationDtoChatDtoMapper.MapToConversationResponseDto(conversationId, response);
        await insertTask;
        return conversationResponseDto;
    }

    private async IAsyncEnumerable<ConversationUpdateResponseDto> HandleStreamingResponseAsync(
        IAsyncEnumerable<ChatResponseUpdate> chatCompletionStreamAsync, string conversationId, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var chatResponseUpdates = new List<ChatResponseUpdate>();
        await foreach (var update in chatCompletionStreamAsync.WithCancellation(cancellationToken))
        {
            Logger.LogTrace("Computing chat completions streaming Update Received: {@Update}", update);
            chatResponseUpdates.Add(update);
            yield return   conversationDtoChatDtoMapper.MapToConversationUpdateResponseDto(conversationId, update);
        }
        var updateConversationWithChatResponseCommand = new UpdateConversationUpdateRoundCommand(conversationId, chatResponseUpdates.ToChatResponse());

        await updateRoundCommandHandler.HandleAsync(updateConversationWithChatResponseCommand, cancellationToken);
    }


}
