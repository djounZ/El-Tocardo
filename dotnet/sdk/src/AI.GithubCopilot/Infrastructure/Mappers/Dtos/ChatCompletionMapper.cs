using Microsoft.Extensions.Logging;
using AI.GithubCopilot.Infrastructure.Dtos.ChatCompletion;
using Microsoft.Extensions.AI;
using System.Text.Json;

namespace AI.GithubCopilot.Infrastructure.Mappers.Dtos;

public sealed class ChatCompletionMapper(ILogger<ChatCompletionMapper> logger)
{
    public record ChatCompletionRequest(IEnumerable<ChatMessage> Messages, ChatOptions? Options);

    public ChatCompletionRequestDto MapToChatCompletionRequestDto(ChatCompletionRequest domainModel)
    {
        logger.LogTrace("Mapping To ChatCompletionRequestDto from ChatCompletionRequest {@ChatRequestDto}", domainModel);

        var chatMessageDtos = MapToChatMessageDtos(domainModel.Messages, domainModel.Options).ToArray();

        return new ChatCompletionRequestDto(
            Messages: chatMessageDtos,
            Model: domainModel.Options?.ModelId,
            Temperature: domainModel.Options?.Temperature,
            MaxTokens: domainModel.Options?.MaxOutputTokens,
            Stream: false,
            TopP: domainModel.Options?.TopP,
            FrequencyPenalty: domainModel.Options?.FrequencyPenalty,
            PresencePenalty: domainModel.Options?.PresencePenalty,
            Stop: domainModel.Options?.StopSequences?.ToArray(),
            User: null,
            N: null,
            LogitBias: null,
            LogProbs: null,
            TopLogProbs: null,
            ResponseFormat: MapToResponseFormatDto(domainModel.Options?.ResponseFormat),
            Seed: (int?)domainModel.Options?.Seed,
            Tools: MapToToolDtos(domainModel.Options?.Tools),
            ToolChoice: MapToToolChoiceDto(domainModel.Options?.ToolMode),
            ParallelToolCalls: domainModel.Options?.AllowMultipleToolCalls
        );
    }

    public ChatCompletionRequest MapToChatCompletionRequest(ChatCompletionRequestDto dto)
    {
        logger.LogTrace("Mapping To ChatCompletionRequest from ChatCompletionRequestDto {@ChatRequest}", dto);

        var messages = dto.Messages.Select(MapToChatMessage).ToArray();
        var options = new ChatOptions
        {
            ModelId = dto.Model,
            Temperature = (float?)dto.Temperature,
            MaxOutputTokens = dto.MaxTokens,
            TopP = (float?)dto.TopP,
            FrequencyPenalty = (float?)dto.FrequencyPenalty,
            PresencePenalty = (float?)dto.PresencePenalty,
            StopSequences = dto.Stop?.ToList(),
            Seed = dto.Seed,
            ResponseFormat = MapToChatResponseFormat(dto.ResponseFormat),
            Tools = MapToTools(dto.Tools),
            ToolMode = MapToChatToolMode(dto.ToolChoice),
            AllowMultipleToolCalls = dto.ParallelToolCalls
        };

        return new ChatCompletionRequest(messages, options);
    }


    public async IAsyncEnumerable<ChatResponseUpdate> MapToChatResponseUpdates(
        IAsyncEnumerable<ChatCompletionResponseDto?> chatCompletionResponseDtos,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? responseId = null;

        await foreach (var chatCompletionResponseDto in chatCompletionResponseDtos.WithCancellation(cancellationToken))
        {
            if (chatCompletionResponseDto != null)
            {
                // Use the first response ID for all updates in the stream
                responseId ??= chatCompletionResponseDto.Id;

                yield return MapToChatResponseUpdate(chatCompletionResponseDto);
            }
            else
            {
                logger.LogWarning("Received null ChatCompletionResponseDto in stream, skipping update.");
            }
        }
    }

    public ChatResponse MapToChatResponse(ChatCompletionResponseDto dto)
    {
        logger.LogTrace("Mapping To ChatResponse from ChatCompletionResponseDto {@ChatRequest}", dto);

        var firstChoice = dto.Choices.FirstOrDefault() ?? throw new InvalidOperationException("Invalid response: no valid choice with message found");
        var message = MapToChatMessage(firstChoice);
        var usageDetails = MapToUsageDetails(dto.Usage);
        var response = new ChatResponse(message)
        {
            ResponseId = dto.Id,
            ModelId = dto.Model,
            CreatedAt = DateTimeOffset.FromUnixTimeSeconds(dto.Created),
            FinishReason = MapToChatFinishReason(firstChoice.FinishReason),
            Usage = usageDetails
        };

        return response;
    }

    private ChatResponseUpdate MapToChatResponseUpdate(ChatCompletionResponseDto dto)
    {

        var chatChoiceDto = dto.Choices[0];

        var messageDto = chatChoiceDto.Message;
        var chatDeltaDto = chatChoiceDto.Delta;

        var contents = new List<AIContent>();
        var usageContent = MapToUsageContent(dto);
        AddContentIfNotNull(usageContent, contents);
        var role = ChatRole.Assistant;
        if (chatDeltaDto != null)
        {
            contents.AddRange(MapToAiContents(chatDeltaDto.Content, chatDeltaDto.ToolCalls));
            role = MapToChatRole(chatDeltaDto.Role, ChatRole.Assistant);
        }
        else if (messageDto != null)
        {
            contents.AddRange( MapToAiContents(messageDto.Content, messageDto.ToolCalls));
            role = MapToChatRole(messageDto.Role, ChatRole.Assistant);
        }
        var update = new ChatResponseUpdate(role, contents)
        {
            ResponseId = dto.Id,
            MessageId = messageDto?.Name,
            ModelId = dto.Model,
            CreatedAt = DateTimeOffset.FromUnixTimeSeconds(dto.Created),
            FinishReason = MapToChatFinishReason(chatChoiceDto.FinishReason),
            RawRepresentation = dto
        };
        return update;
    }

    private static void AddContentIfNotNull(AIContent? content, List<AIContent> contents)
    {
        if (content != null)
        {
            contents.Add(content);
        }
    }

    private static UsageContent? MapToUsageContent(ChatCompletionResponseDto dto)
    {
        var usageDetails = MapToUsageDetails(dto.Usage);
        var usageContent = usageDetails != null ? new UsageContent(usageDetails) { RawRepresentation = dto.Usage } : null;
        return usageContent;
    }

    private static UsageDetails? MapToUsageDetails(UsageDto? usageDto)
    {
        UsageDetails? usageDetails = null;
        if (usageDto != null)
        {
            usageDetails = new UsageDetails
            {
                InputTokenCount = usageDto.PromptTokens,
                OutputTokenCount = usageDto.CompletionTokens,
                TotalTokenCount = usageDto.TotalTokens
            };
        }

        return usageDetails;
    }

    private List<AITool>? MapToTools(IReadOnlyList<ToolDto>? dto)
    {
        return dto?.Select(MapToAiTool).Where(t => t != null).Cast<AITool>().ToList();
    }

    private IEnumerable<ChatMessageDto> MapToChatMessageDtos(IEnumerable<ChatMessage> messages, ChatOptions? options)
    {
        if (!string.IsNullOrWhiteSpace(options?.Instructions))
        {
            yield return new ChatMessageDto(
                Role: "system",
                Content: new TextContentDto(options.Instructions)
            );
        }

        foreach (var message in messages)
        {
            yield return MapToChatMessageDtos(message);
        }
    }

    private ChatMessageDto MapToChatMessageDtos(ChatMessage message)
    {
        var role = message.Role.Value.ToLowerInvariant();
        var content = MapToMessageContentDto(message.Contents);
        var toolCalls = MapToToolCallDtos(message.Contents);
        var toolCallId = ExtractToolCallId(message.Contents);

        return new ChatMessageDto(
            Role: role,
            Content: content,
            Name: message.AuthorName,
            ToolCalls: toolCalls,
            ToolCallId: toolCallId
        );
    }

    private MessageContentDto? MapToMessageContentDto(IList<AIContent> contents)
    {
        var parts = new List<ContentPartDto>();
        var textBuffer = new List<string>();
        bool hasNonText = false;

        foreach (var content in contents)
        {
            switch (content)
            {
                case FunctionCallContent:
                    // handled in tools
                    break;

                case TextContent textContent:
                    if (!string.IsNullOrWhiteSpace(textContent.Text))
                    {
                        textBuffer.Add(textContent.Text);
                    }
                    break;

                case FunctionResultContent resultContent:
                    // For tool results, serialize the result
                    var resultText = resultContent.Result switch
                    {
                        string str => str,
                        null => string.Empty,
                        _ => JsonSerializer.Serialize(resultContent.Result)
                    };

                    if (!string.IsNullOrWhiteSpace(resultText))
                    {
                        textBuffer.Add(resultText);
                    }
                    break;
                case TextReasoningContent reasoningContent:
                    textBuffer.Add("[Reasoning] " + reasoningContent.Text);
                    break;
                case ErrorContent:
                    //textBuffer.Add($"[Error: {errorContent.Message}{(string.IsNullOrEmpty(errorContent.ErrorCode) ? "" : $" (Code: {errorContent.ErrorCode})")}{(string.IsNullOrEmpty(errorContent.Details) ? "" : $" - {errorContent.Details}") }]");
                    break;

                case UriContent uriContent:
                    hasNonText = true;
                    if (uriContent.MediaType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                    {
                        parts.Add(new ImagePartDto(new ImageUrlDto(uriContent.Uri.ToString())));
                    }
                    else
                    {
                        throw new NotSupportedException($"UriContent with media type '{uriContent.MediaType}' is not supported as chat content.");
                    }
                    break;

                case DataContent dataContent:
                    hasNonText = true;
                    if (dataContent.MediaType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                    {
                        parts.Add(new ImagePartDto(new ImageUrlDto(dataContent.Uri)));
                    }
                    else
                    {
                        throw new NotSupportedException($"DataContent with media type '{dataContent.MediaType}' is not supported as chat content.");
                    }
                    break;

                case UsageContent:
                    // Skip usage content
                    break;

                default:
                    throw new NotSupportedException($"AIContent type '{content.GetType().Name}' is not supported in ExtractMessageContent.");
            }
        }

        if (!hasNonText && parts.Count == 0)
        {
            return textBuffer.Count switch
            {
                0 => null,
                1 => new TextContentDto(textBuffer[0]),
                _ => new TextContentDto(string.Join("\n", textBuffer))
            };
        }
        parts.AddRange(textBuffer.Select(t => new TextPartDto(t)));

        return parts.Count > 0
            ? new MultipartContentDto([.. parts])
            : null;
    }

    private IReadOnlyList<ToolCallDto>? MapToToolCallDtos(IList<AIContent> contents)
    {
        var toolCalls = contents.OfType<FunctionCallContent>().Select(MapToToolCallDto).ToList();

        return toolCalls.Count > 0 ? toolCalls : null;
    }

    private static ToolCallDto MapToToolCallDto(FunctionCallContent functionCall)
    {
        var argumentsJson = functionCall.Arguments switch
        {
            { } dict => JsonSerializer.Serialize(dict),
            _ => JsonSerializer.Serialize(functionCall.Arguments)
        };
        var toolCallDto = new ToolCallDto(
            Id: functionCall.CallId,
            Type: "function",
            Function: new FunctionCallDto(
                Name: functionCall.Name,
                Arguments: argumentsJson
            )
        );
        return toolCallDto;
    }

    private string? ExtractToolCallId(IList<AIContent> contents)
    {
        return contents.OfType<FunctionResultContent>().FirstOrDefault()?.CallId;
    }

    private IReadOnlyList<ToolDto>? MapToToolDtos(IEnumerable<AITool>? tools)
    {
        var copilotTools = tools?.OfType<AIFunction>().Select(MapToToolDto).ToList();

        return copilotTools?.Count > 0 ? copilotTools : null;
    }

    private static ToolDto MapToToolDto(AIFunction function)
    {
        var toolDto = new ToolDto(
            Type: "function",
            Function: new FunctionDefinitionDto(
                Name: function.Name,
                Description: function.Description,
                Parameters: function.JsonSchema
            )
        );
        return toolDto;
    }

    private ToolChoiceDto? MapToToolChoiceDto(ChatToolMode? toolMode)
    {
        return toolMode switch
        {
            AutoChatToolMode => ToolChoiceDto.Auto,
            RequiredChatToolMode { RequiredFunctionName: not null } required =>
                ToolChoiceDto.ForFunction(required.RequiredFunctionName),
            RequiredChatToolMode => ToolChoiceDto.Required,
            _ => null
        };
    }


    private ResponseFormatDto? MapToResponseFormatDto(ChatResponseFormat? responseFormat)
    {
        return responseFormat switch
        {
            ChatResponseFormatText => new ResponseFormatDto("text"),
            ChatResponseFormatJson jsonFormat => new ResponseFormatDto(
                "json_object",
                jsonFormat.Schema.HasValue ? new JsonSchemaDto(
                    "object",
                    jsonFormat.SchemaDescription ?? "Generated schema",
                    jsonFormat.Schema.Value
                ) : null
            ),
            _ => null
        };
    }

    private IDictionary<string, object?> MapToIDictionaryStringNullableObject(string? argumentsJson)
    {
        if (string.IsNullOrWhiteSpace(argumentsJson))
        {
            return new Dictionary<string, object?>();
        }

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, object?>>(argumentsJson)
                   ?? new Dictionary<string, object?>();
        }
        catch
        {
            return new Dictionary<string, object?>();
        }
    }

    private ChatMessage MapToChatMessage(ChatChoiceDto dto)
    {
        var messageDto = dto.Message;

        var contents = MapToAiContents(messageDto?.Content, messageDto?.ToolCalls);

        var role =MapToChatRole(messageDto?.Role, ChatRole.Assistant);
        return new ChatMessage(role , contents);
    }


    private List<AIContent> MapToAiContents(string? deltaContent, IReadOnlyList<ToolCallDeltaDto>? toolCallDtos)
    {
        var contents = new List<AIContent>();
        if (!string.IsNullOrEmpty(deltaContent))
        {
            contents.Add(new TextContent(deltaContent));
        }
        var functionCallContents = MapToFunctionCallContents(toolCallDtos);
        contents.AddRange(functionCallContents);
        return contents;
    }

    private IEnumerable<FunctionCallContent> MapToFunctionCallContents(IReadOnlyList<ToolCallDeltaDto>? dtos)
    {
        return dtos?.Any() != true ? [] : dtos.Select(MapToFunctionCallContent);
    }

    private FunctionCallContent MapToFunctionCallContent(ToolCallDeltaDto dto)
    {
        var args = MapToIDictionaryStringNullableObject(dto.Function?.Arguments);
        var functionCallContent = new FunctionCallContent(dto.Id ?? Guid.NewGuid().ToString(), dto.Function?.Name ?? string.Empty, args);
        return functionCallContent;
    }


    private List<AIContent> MapToAiContents(MessageContentDto? messageContentDto, IReadOnlyList<ToolCallDto>? toolCallDtos)
    {
        var contents = new List<AIContent>();
        var textContent = MapToTextContent(messageContentDto);
        if (textContent != null)
        {
            contents.Add(textContent);
        }
        var functionCallContents = MapToFunctionCallContents(toolCallDtos);
        contents.AddRange(functionCallContents);
        return contents;
    }

    private ChatMessage MapToChatMessage(ChatMessageDto dto)
    {
        var role = MapToChatRole(dto.Role, ChatRole.User);

        var contents = new List<AIContent>();

        if (dto.Content is TextContentDto textContent)
        {
            contents.Add(new TextContent(textContent.Text));
        }

        var functionCallContents = MapToFunctionCallContents(dto.ToolCalls);
        contents.AddRange(functionCallContents);

        return new ChatMessage(role, contents);
    }

    private IEnumerable<FunctionCallContent> MapToFunctionCallContents(IReadOnlyList<ToolCallDto>? toolCallDtos)
    {
        return toolCallDtos?.Any() != true ? [] : toolCallDtos.Select(MapToFunctionCallContent);
    }

    private FunctionCallContent MapToFunctionCallContent(ToolCallDto toolCall)
    {
        var args = MapToIDictionaryStringNullableObject(toolCall.Function.Arguments);
        var functionCallContent = new FunctionCallContent(toolCall.Id, toolCall.Function.Name, args);
        return functionCallContent;
    }

    private TextContent? MapToTextContent(MessageContentDto? dto)
    {

        var content = dto switch
        {
            TextContentDto text => text.Text,
            MultipartContentDto multipart =>
                string.Join(" ", multipart.Parts.OfType<TextPartDto>().Select(p => p.Text)),
            _ => null
        };
        return string.IsNullOrEmpty(content) ? null : new TextContent(content);
    }

    private static ChatRole MapToChatRole(string? dto, ChatRole defaultChatRole)
    {
        return dto switch
        {
            "system" => ChatRole.System,
            "user" => ChatRole.User,
            "assistant" => ChatRole.Assistant,
            "tool" => ChatRole.Tool,
            _ => defaultChatRole
        };
    }

    private ChatResponseFormat? MapToChatResponseFormat(ResponseFormatDto? dto)
    {
        return dto?.Type switch
        {
            "text" => ChatResponseFormat.Text,
            "json_object" => dto.JsonSchema != null ?
                new ChatResponseFormatJson(dto.JsonSchema.Schema as JsonElement? ?? default, dto.JsonSchema.Name, dto.JsonSchema.Description) :
                ChatResponseFormat.Json,
            _ => null
        };
    }

    private AITool? MapToAiTool(ToolDto? dto)
    {
        if (dto?.Function == null)
        {
            return null;
        }

        // For now, create a simple function tool without complex parameters
        // This can be enhanced later based on specific requirements
        return null; // Simplified for compilation
    }

    private ChatToolMode? MapToChatToolMode(ToolChoiceDto? dto)
    {
        if (dto == null)
        {
            return null;
        }

        return dto.Type switch
        {
            "auto" => new AutoChatToolMode(),
            "none" => new NoneChatToolMode(),
            "function" => new RequiredChatToolMode(dto.Function?.Name),
            _ => null
        };
    }

    private ChatFinishReason? MapToChatFinishReason(string? reason)
    {
        return reason switch
        {
            "stop" => ChatFinishReason.Stop,
            "length" => ChatFinishReason.Length,
            "tool_calls" => ChatFinishReason.ToolCalls,
            "content_filter" => ChatFinishReason.ContentFilter,
            null => null,
            _ => new ChatFinishReason(reason)
        };
    }
}
