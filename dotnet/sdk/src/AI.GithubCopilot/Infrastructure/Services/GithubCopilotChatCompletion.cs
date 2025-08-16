using System.Text.Json;
using System.Text.Json.Serialization;
using AI.GithubCopilot.Infrastructure.Dtos;
using AI.GithubCopilot.Infrastructure.Dtos.ChatCompletion;
using AI.GithubCopilot.Infrastructure.Models;
using AI.GithubCopilot.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace AI.GithubCopilot.Infrastructure.Services;

public sealed class GithubCopilotChatCompletion(
    ILogger<GithubCopilotChatCompletion> logger,
    HttpClient httpClient,
    IOptions<AiGithubOptions> options,
    HttpClientRunner httpClientRunner)

{
    private AiGithubOptions Options => options.Value;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };


    public async Task<ModelsResponseDto> GetModelsAsync(CancellationToken cancellationToken)
    {
        var response = await httpClientRunner.SendAndDeserializeAsync<ModelsResponseDto>(
            httpClient,
            HttpMethod.Get,
            Options.CopilotModelsUrl,
            Options.CopilotChatCompletionsHeaders,
            HttpCompletionOption.ResponseContentRead,
            JsonOptions,
            cancellationToken,
            logger
        );

        return response;
    }

    public async Task<ChatCompletionResponseDto> GetChatCompletionAsync(
        ChatCompletionRequestDto request,
        CancellationToken cancellationToken)
    {
        // Ensure streaming is disabled
        var streamingRequest = request with { Stream = false };

        var headers = GetCopilotChatCompletionsHeaders(request.Messages);
        return await httpClientRunner.SendAndDeserializeAsync<ChatCompletionRequestDto, ChatCompletionResponseDto>(
            streamingRequest,
            httpClient,
            HttpMethod.Post,
            Options.CopilotChatCompletionsUrl,
            headers,
            HttpCompletionOption.ResponseContentRead,
            JsonOptions,
            cancellationToken,
            logger
        );
    }

    public async IAsyncEnumerable<ChatCompletionResponseDto?> GetChatCompletionStreamAsync(
        ChatCompletionRequestDto request,
       [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {

        // Ensure streaming is enabled
        var streamingRequest = request with { Stream = true };
        ChatCompletionResponseDto? previousResponse = null;

        var headers = GetCopilotChatCompletionsHeaders(request.Messages);

        await foreach (var item in httpClientRunner.SendAndReadStreamAsync(
                           streamingRequest,
                           httpClient,
                           HttpMethod.Post,
                           Options.CopilotChatCompletionsUrl,
                           headers,
                           HttpCompletionOption.ResponseContentRead,
                           JsonOptions,
                           cancellationToken,
                           ReadItemAsync,
                           logger))
        {
            if (item.IsIgnored)
            {
                continue;
            }

            if (item.IsEnded)
            {
                if (previousResponse is not null)
                {
                    // If we have a previous response, yield it before ending

                    logger.LogInformation("Tool call detected in response: {Response}", previousResponse);
                    yield return previousResponse;
                }
                break;
            }

            var itemValue = item.Value;
            if (itemValue != null && IsToolCall(itemValue))
            {
                if (previousResponse == null)
                {
                    previousResponse = itemValue;
                }
                else if (previousResponse.Id != itemValue.Id)
                {
                    // If the ID has changed, yield the previous response
                    yield return previousResponse;

                    logger.LogInformation("Tool call detected in response: {Response}", previousResponse);
                    previousResponse = itemValue;
                }
                else
                {
                    // If the ID is the same, we are still processing the same response
                    previousResponse = AccumulateToolCallArguments(previousResponse, itemValue);
                }
                continue;
            }
            yield return itemValue;
        }
    }

    private Dictionary<string, string> GetCopilotChatCompletionsHeaders(IReadOnlyList<ChatMessageDto> messages)
    {
        var headers = Options.CopilotChatCompletionsHeaders;

        if (messages.Any(m => m.Content is MultipartContentDto multipartContent && multipartContent.Parts.Any(contentPart => contentPart is ImagePartDto)))
        {

            headers = new Dictionary<string, string>(headers)
            {
                { "Copilot-Vision-Request", "true" }
            };

        }

        return headers;
    }

    private ChatCompletionResponseDto AccumulateToolCallArguments(ChatCompletionResponseDto previousResponse, ChatCompletionResponseDto itemValue)
    {
        // If the ID is the same, we are still processing the same response
        var functionArguments = itemValue.Choices.Single().Delta?.ToolCalls?.Single().Function?.Arguments ?? string.Empty;

        // Get the existing function arguments and append the new ones
        var existingChoice = previousResponse.Choices.Single();
        var existingToolCall = existingChoice.Delta?.ToolCalls?.Single();
        var existingArguments = existingToolCall?.Function?.Arguments ?? string.Empty;

        if (existingToolCall?.Function != null && existingChoice.Delta != null)
        {
            // Create updated tool call with accumulated arguments
            var updatedFunction = existingToolCall.Function with
            {
                Arguments = existingArguments + functionArguments
            };

            var updatedToolCall = existingToolCall with { Function = updatedFunction };
            var updatedDelta = existingChoice.Delta with
            {
                ToolCalls = [updatedToolCall]
            };
            var updatedChoice = existingChoice with { Delta = updatedDelta };

            return previousResponse with
            {
                Choices = [updatedChoice]
            };
        }
        return previousResponse;
    }
    private bool IsToolCall(ChatCompletionResponseDto itemValue)
    {
        if (itemValue.Choices.Count == 0)
        {
            return false;
        }

        var choice = itemValue.Choices[0];
        if (choice.Delta is null)
        {
            return false;
        }

        return choice.Delta.ToolCalls != null && choice.Delta.ToolCalls.Count != 0;
    }

    private async Task<StreamItem<ChatCompletionResponseDto>> ReadItemAsync(StreamReader reader, CancellationToken o)
    {
        var line = await reader.ReadLineAsync(o);
        logger.LogTrace(line);
        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("event:") || line.StartsWith($":"))
        {
            return StreamItem<ChatCompletionResponseDto>.BuildIgnored(line);
        }

        var jsonLine = line.StartsWith("data: ") ? line[6..] : line;

        if (jsonLine == "[DONE]")
        {
            return StreamItem<ChatCompletionResponseDto>.BuildEnded(line);
        }

        return StreamItem<ChatCompletionResponseDto>.BuildContent(
            JsonSerializer.Deserialize<ChatCompletionResponseDto>(jsonLine, JsonOptions)!);
    }

}
