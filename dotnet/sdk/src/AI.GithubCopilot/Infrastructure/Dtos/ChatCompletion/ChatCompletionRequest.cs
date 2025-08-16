using System.Text.Json;
using System.Text.Json.Serialization;

namespace AI.GithubCopilot.Infrastructure.Dtos.ChatCompletion;

public record ChatCompletionRequestDto(
    [property: JsonPropertyName("messages")] IReadOnlyList<ChatMessageDto> Messages,
    [property: JsonPropertyName("model")] string? Model = null,
    [property: JsonPropertyName("temperature")] double? Temperature = null,
    [property: JsonPropertyName("max_tokens")] int? MaxTokens = null,
    [property: JsonPropertyName("stream")] bool Stream = false,
    [property: JsonPropertyName("top_p")] double? TopP = null,
    [property: JsonPropertyName("frequency_penalty")] double? FrequencyPenalty = null,
    [property: JsonPropertyName("presence_penalty")] double? PresencePenalty = null,
    [property: JsonPropertyName("stop")] IReadOnlyList<string>? Stop = null,
    [property: JsonPropertyName("user")] string? User = null,
    [property: JsonPropertyName("n")] int? N = null,

    [property: JsonPropertyName("logit_bias")] Dictionary<string, double>? LogitBias = null,
    [property: JsonPropertyName("logprobs")] bool? LogProbs = null,
    [property: JsonPropertyName("top_logprobs")] int? TopLogProbs = null,
    [property: JsonPropertyName("response_format")] ResponseFormatDto? ResponseFormat = null,
    [property: JsonPropertyName("seed")] int? Seed = null,
    [property: JsonPropertyName("tools")] IReadOnlyList<ToolDto>? Tools = null,
    [property: JsonPropertyName("tool_choice")] ToolChoiceDto? ToolChoice = null,
    [property: JsonPropertyName("parallel_tool_calls")] bool? ParallelToolCalls = null
);

[JsonConverter(typeof(ToolChoiceConverter))]
public record ToolChoiceDto
{
    public string? Type { get; init; }
    public ToolFunctionDto? Function { get; init; }

    public static implicit operator ToolChoiceDto(string value) => new() { Type = value };

    public static ToolChoiceDto Auto => new() { Type = "auto" };
    public static ToolChoiceDto None => new() { Type = "none" };
    public static ToolChoiceDto Required => new() { Type = "required" };

    public static ToolChoiceDto ForFunction(string name) => new()
    {
        Type = "function",
        Function = new ToolFunctionDto(name)
    };
}

/// <summary>
/// Tool function specification for specific function selection
/// </summary>
public record ToolFunctionDto(
    [property: JsonPropertyName("name")] string Name
);

/// <summary>
/// Converter for ToolChoice to handle both string and object formats
/// </summary>
public class ToolChoiceConverter : JsonConverter<ToolChoiceDto>
{
    public override ToolChoiceDto Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return new ToolChoiceDto { Type = reader.GetString() };
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            var type = root.TryGetProperty("type", out var typeElement) ? typeElement.GetString() : null;
            ToolFunctionDto? function = null;

            if (root.TryGetProperty("function", out var functionElement))
            {
                var name = functionElement.TryGetProperty("name", out var nameElement) ? nameElement.GetString() : null;
                if (name != null)
                {
                    function = new ToolFunctionDto(name);
                }
            }

            return new ToolChoiceDto { Type = type, Function = function };
        }

        throw new JsonException("ToolChoice must be a string or object");
    }

    public override void Write(Utf8JsonWriter writer, ToolChoiceDto value, JsonSerializerOptions options)
    {
        if (value.Function == null)
        {
            writer.WriteStringValue(value.Type);
        }
        else
        {
            writer.WriteStartObject();
            writer.WriteString("type", value.Type);
            writer.WritePropertyName("function");
            writer.WriteStartObject();
            writer.WriteString("name", value.Function.Name);
            writer.WriteEndObject();
            writer.WriteEndObject();
        }
    }
}

public record ResponseFormatDto(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("json_schema")] JsonSchemaDto? JsonSchema = null
);
public record JsonSchemaDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description = null,
    [property: JsonPropertyName("schema")] object? Schema = null,
    [property: JsonPropertyName("strict")] bool? Strict = null
);

public record ChatMessageDto(
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("content")] MessageContentDto? Content = null,
    [property: JsonPropertyName("name")] string? Name = null,
    [property: JsonPropertyName("tool_calls")] IReadOnlyList<ToolCallDto>? ToolCalls = null,
    [property: JsonPropertyName("tool_call_id")] string? ToolCallId = null,
    [property: JsonPropertyName("refusal")] string? Refusal = null
);

public record ToolCallDto(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("function")] FunctionCallDto Function
);


public record FunctionCallDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("arguments")] string Arguments
);

[JsonConverter(typeof(MessageContentConverter))]
public abstract record MessageContentDto();



public record TextContentDto(string Text) : MessageContentDto;


public record MultipartContentDto(ContentPartDto[] Parts) : MessageContentDto;


[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(TextPartDto), typeDiscriminator: "text")]
[JsonDerivedType(typeof(ImagePartDto), typeDiscriminator: "image_url")]
public abstract record ContentPartDto;

public sealed record TextPartDto(
    [property: JsonPropertyName("text")] string Text
) : ContentPartDto;

public sealed record ImagePartDto(
    [property: JsonPropertyName("image_url")] ImageUrlDto ImageUrl
) : ContentPartDto;

public record ImageUrlDto(
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("detail")] string? Detail = null
);

public class MessageContentConverter : JsonConverter<MessageContentDto>
{
    public override MessageContentDto Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return new TextContentDto(reader.GetString()!);
        }

        if (reader.TokenType == JsonTokenType.StartArray)
        {
            var parts = JsonSerializer.Deserialize<ContentPartDto[]>(ref reader, options);
            return new MultipartContentDto(parts ?? []);
        }

        throw new JsonException("MessageContent must be a string or array");
    }

    public override void Write(Utf8JsonWriter writer, MessageContentDto value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case TextContentDto text:
                writer.WriteStringValue(text.Text);
                break;
            case MultipartContentDto multipart:
                JsonSerializer.Serialize(writer, multipart.Parts, options);
                break;
            default:
                throw new JsonException($"Unknown MessageContent type: {value.GetType()}");
        }
    }
}
public record ChatCompletionResponseDto(
    [property: JsonPropertyName("choices")] IReadOnlyList<ChatChoiceDto> Choices,
    [property: JsonPropertyName("created")] long Created,
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("object")] string Object,
    [property: JsonPropertyName("model")] string? Model = null,
    [property: JsonPropertyName("system_fingerprint")] string? SystemFingerprint = null,
    [property: JsonPropertyName("usage")] UsageDto? Usage = null,
    [property: JsonPropertyName("prompt_filter_results")] IReadOnlyList<PromptFilterResultDto>? PromptFilterResults = null
);
public record PromptFilterResultDto(
    [property: JsonPropertyName("prompt_index")] int PromptIndex,
    [property: JsonPropertyName("content_filter_results")] ContentFilterResultsDto ContentFilterResults
);

public record UsageDto(
    [property: JsonPropertyName("completion_tokens")] int CompletionTokens,
    [property: JsonPropertyName("prompt_tokens")] int PromptTokens,
    [property: JsonPropertyName("total_tokens")] int TotalTokens,
    [property: JsonPropertyName("completion_tokens_details")] CompletionTokensDetailsDto? CompletionTokensDetails = null,
    [property: JsonPropertyName("prompt_tokens_details")] PromptTokensDetailsDto? PromptTokensDetails = null
);public record PromptTokensDetailsDto(
    [property: JsonPropertyName("cached_tokens")] int CachedTokens
);

public record CompletionTokensDetailsDto(
    [property: JsonPropertyName("reasoning_tokens")] int? ReasoningTokens = null,
    [property: JsonPropertyName("accepted_prediction_tokens")] int? AcceptedPredictionTokens = null,
    [property: JsonPropertyName("rejected_prediction_tokens")] int? RejectedPredictionTokens = null
);

public record ChatChoiceDto(
    [property: JsonPropertyName("index")] int Index,
    [property: JsonPropertyName("message")] ChatMessageDto? Message = null,
    [property: JsonPropertyName("delta")] ChatDeltaDto? Delta = null,
    [property: JsonPropertyName("finish_reason")] string? FinishReason = null,
    [property: JsonPropertyName("logprobs")] LogProbabilityInfoDto? LogProbs = null,
    [property: JsonPropertyName("content_filter_results")] ContentFilterResultsDto? ContentFilterResults = null,
    [property: JsonPropertyName("content_filter_offsets")] ContentFilterOffsetsDto? ContentFilterOffsets = null
);
public record ContentFilterOffsetsDto(
    [property: JsonPropertyName("check_offset")] int CheckOffset,
    [property: JsonPropertyName("start_offset")] int StartOffset,
    [property: JsonPropertyName("end_offset")] int EndOffset
);

public record ContentFilterResultsDto(
    [property: JsonPropertyName("hate")] ContentFilterResultDto Hate,
    [property: JsonPropertyName("self_harm")] ContentFilterResultDto SelfHarm,
    [property: JsonPropertyName("sexual")] ContentFilterResultDto Sexual,
    [property: JsonPropertyName("violence")] ContentFilterResultDto Violence
);
public record ContentFilterResultDto(
    [property: JsonPropertyName("filtered")] bool Filtered,
    [property: JsonPropertyName("severity")] string Severity
);

public record LogProbabilityInfoDto(
    [property: JsonPropertyName("tokens")] IReadOnlyList<string> Tokens,
    [property: JsonPropertyName("token_logprobs")] IReadOnlyList<double> TokenLogProbs,
    [property: JsonPropertyName("top_logprobs")] IReadOnlyList<IReadOnlyDictionary<string, double>> TopLogProbs,
    [property: JsonPropertyName("text_offset")] IReadOnlyList<int> TextOffset
);

public record ChatDeltaDto(
    [property: JsonPropertyName("content")] string? Content = null,
    [property: JsonPropertyName("role")] string? Role = null,
    [property: JsonPropertyName("tool_calls")] IReadOnlyList<ToolCallDeltaDto>? ToolCalls = null,
    [property: JsonPropertyName("refusal")] string? Refusal = null
);
public record ToolCallDeltaDto(
    [property: JsonPropertyName("index")] int Index,
    [property: JsonPropertyName("id")] string? Id = null,
    [property: JsonPropertyName("type")] string? Type = null,
    [property: JsonPropertyName("function")] FunctionCallDeltaDto? Function = null
);

public record FunctionCallDeltaDto(
    [property: JsonPropertyName("name")] string? Name = null,
    [property: JsonPropertyName("arguments")] string? Arguments = null
);

public record ToolDto(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("function")] FunctionDefinitionDto Function
);
public record FunctionDefinitionDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description = null,
    [property: JsonPropertyName("parameters")] object? Parameters = null,
    [property: JsonPropertyName("strict")] bool? Strict = null
);
