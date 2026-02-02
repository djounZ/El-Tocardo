using ElTocardo.Application.Dtos.Conversation;
using ElTocardo.Application.Services;

namespace ElTocardo.API.Endpoints;

public static class ConversationEndpoints
{
    private static string Tags => "Conversations";

    public static IEndpointRouteBuilder MapConversationEndpoints(this IEndpointRouteBuilder app)
    {
        // Start Conversation (non-streaming)
        app.MapPost("/v1/conversation/start", async (
                IConversationEndpointService conversationService,
                StartConversationRequestDto startConversationRequestDto,
                CancellationToken cancellationToken) =>
            {
                var response = await conversationService.StartConversationAsync(startConversationRequestDto, cancellationToken);
                return Results.Ok(response);
            })
            .WithName("StartConversation")
            .WithSummary("Start Conversation")
            .WithDescription("Starts a new conversation (non-streaming)")
            .WithTags(Tags)
            .Accepts<StartConversationRequestDto>("application/json")
            .Produces<ConversationResponseDto>()
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                // Per-endpoint tweaks


                return Task.CompletedTask;
            });

        // Start Conversation (streaming)
        app.MapPost("/v1/conversation/start/stream", (
                IConversationEndpointService conversationService,
                StartConversationRequestDto startConversationRequestDto,
                CancellationToken cancellationToken) =>
            {
                var streamingResponseAsync = conversationService.StartStreamingConversationAsync(startConversationRequestDto, cancellationToken);
                return streamingResponseAsync.ToResult(cancellationToken);
            })
            .WithName("StartStreamingConversation")
            .WithSummary("Start Conversation (Streaming)")
            .WithDescription("Starts a new conversation (streaming)")
            .WithTags(Tags)
            .Accepts<StartConversationRequestDto>("application/json")
            .Produces<ConversationUpdateResponseDto[]>()
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                // Per-endpoint tweaks


                return Task.CompletedTask;
            });

        // Continue Conversation (non-streaming)
        app.MapPost("/v1/conversation/continue", async (
                IConversationEndpointService conversationService,
                ContinueConversationDto continueConversationDto,
                CancellationToken cancellationToken) =>
            {
                var response = await conversationService.ContinueConversationAsync(continueConversationDto, cancellationToken);
                return Results.Ok(response);
            })
            .WithName("ContinueConversation")
            .WithSummary("Continue Conversation")
            .WithDescription("Continues an existing conversation (non-streaming)")
            .WithTags(Tags)
            .Accepts<ContinueConversationDto>("application/json")
            .Produces<ConversationResponseDto>()
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                // Per-endpoint tweaks


                return Task.CompletedTask;
            });

        // Continue Conversation (streaming)
        app.MapPost("/v1/conversation/continue/stream", (
                IConversationEndpointService conversationService,
                ContinueConversationDto continueConversationDto,
                CancellationToken cancellationToken) =>
            {
                var streamingResponseAsync = conversationService.ContinueStreamingConversationAsync(continueConversationDto, cancellationToken);
                return streamingResponseAsync.ToResult(cancellationToken);
            })
            .WithName("ContinueStreamingConversation")
            .WithSummary("Continue Conversation (Streaming)")
            .WithDescription("Continues an existing conversation (streaming)")
            .WithTags(Tags)
            .Accepts<ContinueConversationDto>("application/json")
            .Produces<ConversationUpdateResponseDto[]>()
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                // Per-endpoint tweaks


                return Task.CompletedTask;
            });

        // Get Conversation by ID
        app.MapGet("/v1/conversations/{conversationId}", async (
                IConversationEndpointService conversationService,
                string conversationId,
                CancellationToken cancellationToken) =>
            {
                var result = await conversationService.GetConversation(conversationId, cancellationToken);
                return result.IsSuccess ? Results.Ok(result.ReadValue()) : Results.NotFound();
            })
            .WithName("GetConversation")
            .WithSummary("Get Conversation by ID")
            .WithDescription("Retrieves a conversation by its ID.")
            .WithTags(Tags)
            .Produces<ConversationDto>()
            .Produces(404)
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                // Per-endpoint tweaks


                return Task.CompletedTask;
            });


        // Get All Conversations
        app.MapGet("/v1/conversations", async (
                IConversationEndpointService conversationService,
                CancellationToken cancellationToken) =>
            {
                var result = await conversationService.GetConversations(cancellationToken);
                return result.IsSuccess ? Results.Ok(result.ReadValue()) : Results.InternalServerError(result.ReadError());
            })
            .WithName("GetAllConversations")
            .WithSummary("Get All Conversations")
            .WithDescription("Retrieves All conversation")
            .WithTags(Tags)
            .Produces<ConversationSummaryDto[]>()
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                // Per-endpoint tweaks


                return Task.CompletedTask;
            });



        app.MapDelete("/v1/conversations/{conversationId}",
                async ( IConversationEndpointService conversationService,
                    string conversationId,
                    CancellationToken cancellationToken) =>
                {
                    var result = await conversationService.DeleteConversationAsync(conversationId, cancellationToken);
                    return result.IsSuccess ? Results.NoContent() : Results.NotFound(result.ReadError());
                })
            .WithName("DeleteConversation")
            .WithSummary("Delete Conversation")
            .WithDescription("Deletes a Conversation by Id")
            .WithTags(Tags)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                // Per-endpoint tweaks


                return Task.CompletedTask;
            });
        return app;
    }
}
