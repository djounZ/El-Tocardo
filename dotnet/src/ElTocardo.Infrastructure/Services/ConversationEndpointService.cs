using System.Runtime.CompilerServices;
using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Dtos.Conversation;
using ElTocardo.Application.Mappers.Dtos.AI;
using ElTocardo.Application.Mappers.Dtos.Conversation;
using ElTocardo.Application.Mediator.Common.Interfaces;
using ElTocardo.Application.Mediator.ConversationMediator.Commands;
using ElTocardo.Application.Services;
using ElTocardo.Domain.Mediator.ConversationMediator.Entities;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.Services;

public sealed class ConversationEndpointService(ILogger<ConversationEndpointService> logger,
    ICommandHandler<CreateConversationCommand, string> createConversationCommandHandler,
    ICommandHandler<UpdateConversationUpdateRoundCommand,Conversation> addNewRoundCommandHandler,
    ICommandHandler<UpdateConversationAddNewRoundCommand,Conversation> updateRoundCommandHandler,
    AiChatCompletionMapper aiChatCompletionMapper,
    ChatClientProvider clientProvider,
    AiToolsProviderService aiToolsProviderService,
    ConversationDtoChatDtoMapper conversationDtoChatDtoMapper) : AbstractChatCompletionsService(logger, aiChatCompletionMapper, clientProvider, aiToolsProviderService),IConversationEndpointService
{
    public async Task<ConversationResponseDto> StartConversationAsync(StartConversationRequestDto startConversationRequestDto,
        CancellationToken cancellationToken)
    {


        var chatMessage = AiChatCompletionMapper.MapToChatMessage(startConversationRequestDto.InitialUserMessage);
        var options = AiChatCompletionMapper.MapToChatOptions(startConversationRequestDto.Options);
        await MapTools(startConversationRequestDto.Options, options, cancellationToken);

        var createConversationCommand = new CreateConversationCommand(startConversationRequestDto.Title, startConversationRequestDto.Description, chatMessage, options, startConversationRequestDto.InitialProvider?.ToString());
        var conversationCreation = await createConversationCommandHandler.HandleAsync(createConversationCommand, cancellationToken);

        var conversationId = conversationCreation.ReadValue();
        logger.LogInformation("Starting conversation with Request: {@Request} and Conversation Id: {ConversationId}", startConversationRequestDto, conversationId);

        var chatClient = await ClientProvider.GetChatClientAsync(startConversationRequestDto.InitialProvider, cancellationToken);
        var response = await chatClient.GetResponseAsync([chatMessage], options, cancellationToken);

        var updateConversationWithChatResponseCommand = new UpdateConversationUpdateRoundCommand(conversationId, response);
        var insertTask =addNewRoundCommandHandler.HandleAsync(updateConversationWithChatResponseCommand, cancellationToken);

        var conversationResponseDto = conversationDtoChatDtoMapper.MapToConversationResponseDto(conversationId, response);
        logger.LogTrace("First Response Completed for Conversation Id : {ConversationId}", conversationId);
        await insertTask;
        return conversationResponseDto;
    }

    public async IAsyncEnumerable<ConversationUpdateResponseDto> StartStreamingConversationAsync(StartConversationRequestDto startConversationRequestDto,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {

        var chatMessage = AiChatCompletionMapper.MapToChatMessage(startConversationRequestDto.InitialUserMessage);
        var options = AiChatCompletionMapper.MapToChatOptions(startConversationRequestDto.Options);
        await MapTools(startConversationRequestDto.Options, options, cancellationToken);

        var createConversationCommand = new CreateConversationCommand(startConversationRequestDto.Title, startConversationRequestDto.Description, chatMessage, options, startConversationRequestDto.InitialProvider?.ToString());
        var conversationCreation = await createConversationCommandHandler.HandleAsync(createConversationCommand, cancellationToken);

        var conversationId = conversationCreation.ReadValue();
        logger.LogInformation("Starting Streaming conversation with Request: {@Request} and Conversation Id: {ConversationId}", startConversationRequestDto, conversationId);



        var chatClient = await ClientProvider.GetChatClientAsync(startConversationRequestDto.InitialProvider, cancellationToken);

        var chatCompletionStreamAsync = chatClient.GetStreamingResponseAsync(chatMessage, options, cancellationToken);


        var chatResponseUpdates = new List<ChatResponseUpdate>();
        await foreach (var update in chatCompletionStreamAsync)
        {
            Logger.LogTrace("Computing chat completions streaming Update Received: {@Update}", update);
            chatResponseUpdates.Add(update);
            yield return   conversationDtoChatDtoMapper.MapToConversationUpdateResponseDto(conversationId, update);
        }
        logger.LogTrace("First Streaming Response Completed for Conversation Id : {ConversationId}", conversationId);
        var updateConversationWithChatResponseCommand = new UpdateConversationUpdateRoundCommand(conversationId, chatResponseUpdates.ToChatResponse());

        await addNewRoundCommandHandler.HandleAsync(updateConversationWithChatResponseCommand, cancellationToken);
    }

    public async Task<ConversationResponseDto> ContinueConversationAsync(ContinueConversationDto continueConversationDto, CancellationToken cancellationToken)
    {
        var conversationId = continueConversationDto.ConversationId;

        var chatMessage = AiChatCompletionMapper.MapToChatMessage(continueConversationDto.UserMessage);
        var options = AiChatCompletionMapper.MapToChatOptions(continueConversationDto.Options);

        await MapTools(continueConversationDto.Options, options, cancellationToken);
        var newRoundCommand = new UpdateConversationAddNewRoundCommand(conversationId, chatMessage, options, continueConversationDto.Provider?.ToString());
        var newRoundResult = await updateRoundCommandHandler.HandleAsync(newRoundCommand, cancellationToken);

        logger.LogInformation("Continue conversation with Request: {@Request} for Conversation Id: {ConversationId}", continueConversationDto, conversationId);

        var conversation = newRoundResult.ReadValue();

        var request = new AiChatCompletionMapper.AiChatClientRequest(conversation.Rounds.SelectMany(m=> m.GetAllMessages()), conversation.CurrentOptions);

        var chatClient = await ClientProvider.GetChatClientAsync(Enum.Parse<AiProviderEnumDto>(conversation.CurrentProvider), cancellationToken);
        var response = await chatClient.GetResponseAsync(request.Messages, request.Options, cancellationToken);

        var updateConversationWithChatResponseCommand = new UpdateConversationUpdateRoundCommand(conversationId, response);
        var insertTask =addNewRoundCommandHandler.HandleAsync(updateConversationWithChatResponseCommand, cancellationToken);


        var conversationResponseDto = conversationDtoChatDtoMapper.MapToConversationResponseDto(conversationId, response);
        logger.LogTrace("Response Received for Conversation Id : {ConversationId}", conversationId);
        await insertTask;
        return conversationResponseDto;
    }

    public async IAsyncEnumerable<ConversationUpdateResponseDto> ContinueStreamingConversationAsync(ContinueConversationDto continueConversationDto,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var conversationId = continueConversationDto.ConversationId;

        var chatMessage = AiChatCompletionMapper.MapToChatMessage(continueConversationDto.UserMessage);
        var options = AiChatCompletionMapper.MapToChatOptions(continueConversationDto.Options);

        await MapTools(continueConversationDto.Options, options, cancellationToken);
        var newRoundCommand = new UpdateConversationAddNewRoundCommand(conversationId, chatMessage, options, continueConversationDto.Provider?.ToString());
        var newRoundResult = await updateRoundCommandHandler.HandleAsync(newRoundCommand, cancellationToken);

        logger.LogInformation("Continue streaming conversation with Request: {@Request} for Conversation Id: {ConversationId}", continueConversationDto, conversationId);

        var conversation = newRoundResult.ReadValue();

        var request = new AiChatCompletionMapper.AiChatClientRequest(conversation.Rounds.SelectMany(m=> m.GetAllMessages()), conversation.CurrentOptions);

        var chatClient = await ClientProvider.GetChatClientAsync(Enum.Parse<AiProviderEnumDto>(conversation.CurrentProvider), cancellationToken);
        var chatCompletionStreamAsync = chatClient.GetStreamingResponseAsync(request.Messages, request.Options, cancellationToken);

        var chatResponseUpdates = new List<ChatResponseUpdate>();
        await foreach (var update in chatCompletionStreamAsync)
        {
            Logger.LogTrace("Computing chat completions streaming Update Received: {@Update}", update);
            chatResponseUpdates.Add(update);
            yield return   conversationDtoChatDtoMapper.MapToConversationUpdateResponseDto(conversationId, update);
        }

        logger.LogTrace("Streaming Response Completed for Conversation Id : {ConversationId}", conversationId);
        var updateConversationWithChatResponseCommand = new UpdateConversationUpdateRoundCommand(conversationId, chatResponseUpdates.ToChatResponse());

        await addNewRoundCommandHandler.HandleAsync(updateConversationWithChatResponseCommand, cancellationToken);
    }

}
