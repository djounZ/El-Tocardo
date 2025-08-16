using System.Text.Json.Serialization;

namespace AI.GithubCopilot.Infrastructure.Dtos;

/// <summary>
/// Response containing a list of available models
/// </summary>
public record ModelsResponseDto(
    [property: JsonPropertyName("data")] IReadOnlyList<ModelInfoDto> Data,
    [property: JsonPropertyName("object")] string Object
);

/// <summary>
/// Model information as returned by the models API
/// </summary>
public record ModelInfoDto(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("object")] string Object,
    [property: JsonPropertyName("version")] string Version,
    [property: JsonPropertyName("vendor")] string Vendor,
    [property: JsonPropertyName("preview")] bool Preview,
    [property: JsonPropertyName("is_chat_default")] bool IsChatDefault,
    [property: JsonPropertyName("is_chat_fallback")] bool IsChatFallback,
    [property: JsonPropertyName("model_picker_enabled")] bool ModelPickerEnabled,
    [property: JsonPropertyName("billing")] BillingInfoDto Billing,
    [property: JsonPropertyName("capabilities")] ModelCapabilitiesDto Capabilities,
    [property: JsonPropertyName("policy")] ModelPolicyDto? Policy = null
);

public record BillingInfoDto(
    [property: JsonPropertyName("is_premium")] bool IsPremium,
    [property: JsonPropertyName("multiplier")] decimal Multiplier,
    [property: JsonPropertyName("restricted_to")] IReadOnlyList<string>? RestrictedTo = null
);

public record ModelCapabilitiesDto(
    [property: JsonPropertyName("family")] string Family,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("tokenizer")] string Tokenizer,
    [property: JsonPropertyName("object")] string Object,
    [property: JsonPropertyName("limits")] ModelLimitsDto Limits,
    [property: JsonPropertyName("supports")] ModelSupportsDto Supports
);

public record ModelLimitsDto(
    [property: JsonPropertyName("max_context_window_tokens")] int? MaxContextWindowTokens = null,
    [property: JsonPropertyName("max_output_tokens")] int? MaxOutputTokens = null,
    [property: JsonPropertyName("max_prompt_tokens")] int? MaxPromptTokens = null,
    [property: JsonPropertyName("max_inputs")] int? MaxInputs = null,
    [property: JsonPropertyName("vision")] ModelVisionLimitsDto? Vision = null
);

public record ModelVisionLimitsDto(
    [property: JsonPropertyName("max_prompt_image_size")] int? MaxPromptImageSize = null,
    [property: JsonPropertyName("max_prompt_images")] int? MaxPromptImages = null,
    [property: JsonPropertyName("supported_media_types")] IReadOnlyList<string>? SupportedMediaTypes = null
);

public record ModelSupportsDto(
    [property: JsonPropertyName("parallel_tool_calls")] bool? ParallelToolCalls = null,
    [property: JsonPropertyName("streaming")] bool? Streaming = null,
    [property: JsonPropertyName("structured_outputs")] bool? StructuredOutputs = null,
    [property: JsonPropertyName("tool_calls")] bool? ToolCalls = null,
    [property: JsonPropertyName("vision")] bool? Vision = null,
    [property: JsonPropertyName("dimensions")] bool? Dimensions = null
);

public record ModelPolicyDto(
    [property: JsonPropertyName("state")] string State,
    [property: JsonPropertyName("terms")] string? Terms = null
);
