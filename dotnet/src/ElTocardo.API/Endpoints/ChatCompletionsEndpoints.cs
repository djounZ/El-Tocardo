using ElTocardo.Application.Dtos.AI.ChatCompletion.Request;
using ElTocardo.Application.Dtos.AI.ChatCompletion.Response;
using ElTocardo.Application.Services;

namespace ElTocardo.API.Endpoints;

public static class ChatCompletionsEndpoints
{
    private static string Tags => "Chat Completions";

    public static IEndpointRouteBuilder MapChatCompletionsEndpoints(this IEndpointRouteBuilder app)
    {

        // provider
        app.MapPost("/v1/chat/completions", async (
                IChatCompletionsEndpointService completionsService,
                ChatRequestDto chatRequestDto,
                CancellationToken cancellationToken) =>
            {
                var response = await completionsService.ComputeChatCompletionsAsync(chatRequestDto, cancellationToken);
                return Results.Ok(response);
            })
            .WithName("ComputeChatCompletions")
            .WithSummary("Chat Completions")
            .WithDescription("Creates a non-streaming chat completions")
            .WithTags(Tags)
            .Accepts<ChatRequestDto>("application/json")
            .Produces<ChatResponseDto>()
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                // Per-endpoint tweaks


                return Task.CompletedTask;
            });

        app.MapPost("/v1/chat/completions/stream", (
                IChatCompletionsEndpointService completionsService,
                ChatRequestDto chatRequestDto,
                CancellationToken cancellationToken) =>
            {
                var streamingResponseAsync = completionsService.ComputeStreamingChatCompletionAsync(chatRequestDto, cancellationToken);
                return streamingResponseAsync.ToResult(cancellationToken);
            })
            .WithName("ComputeStreamingChatCompletion")
            .WithSummary("Chat Completions")
            .WithDescription("Creates a streaming chat completions")
            .WithTags(Tags)
            .Accepts<ChatRequestDto>("application/json")
            .Produces<ChatResponseUpdateDto[]>()
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                // Per-endpoint tweaks


                return Task.CompletedTask;
            });
        return app;
    }

}
