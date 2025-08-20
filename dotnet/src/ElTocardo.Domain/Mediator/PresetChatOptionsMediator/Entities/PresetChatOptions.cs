using ElTocardo.Domain.Mediator.Common.Entities;

namespace ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;

public class PresetChatOptions : AbstractEntity<string>
{
    private PresetChatOptions() { } // EF Core constructor

    public PresetChatOptions(
        string name,
        string? conversationId,
        string? instructions,
        float? temperature,
        int? maxOutputTokens,
        float? topP,
        int? topK,
        float? frequencyPenalty,
        float? presencePenalty,
        long? seed,
        string? responseFormat,
        string? modelId,
        string? stopSequences,
        bool? allowMultipleToolCalls,
        string? toolMode,
        string? tools)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or empty", nameof(name));
        }

        Id = Guid.NewGuid();
        Name = name;
        ConversationId = conversationId;
        Instructions = instructions;
        Temperature = temperature;
        MaxOutputTokens = maxOutputTokens;
        TopP = topP;
        TopK = topK;
        FrequencyPenalty = frequencyPenalty;
        PresencePenalty = presencePenalty;
        Seed = seed;
        ResponseFormat = responseFormat;
        ModelId = modelId;
        StopSequences = stopSequences;
        AllowMultipleToolCalls = allowMultipleToolCalls;
        ToolMode = toolMode;
        Tools = tools;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public string Name { get; private set; } = string.Empty;
    public string? ConversationId { get; private set; }
    public string? Instructions { get; private set; }
    public float? Temperature { get; private set; }
    public int? MaxOutputTokens { get; private set; }
    public float? TopP { get; private set; }
    public int? TopK { get; private set; }
    public float? FrequencyPenalty { get; private set; }
    public float? PresencePenalty { get; private set; }
    public long? Seed { get; private set; }
    public string? ResponseFormat { get; private set; }
    public string? ModelId { get; private set; }
    public string? StopSequences { get; private set; }
    public bool? AllowMultipleToolCalls { get; private set; }
    public string? ToolMode { get; private set; }
    public string? Tools { get; private set; }

    public void Update(
        string? conversationId,
        string? instructions,
        float? temperature,
        int? maxOutputTokens,
        float? topP,
        int? topK,
        float? frequencyPenalty,
        float? presencePenalty,
        long? seed,
        string? responseFormat,
        string? modelId,
        string? stopSequences,
        bool? allowMultipleToolCalls,
        string? toolMode,
        string? tools)
    {
        ConversationId = conversationId;
        Instructions = instructions;
        Temperature = temperature;
        MaxOutputTokens = maxOutputTokens;
        TopP = topP;
        TopK = topK;
        FrequencyPenalty = frequencyPenalty;
        PresencePenalty = presencePenalty;
        Seed = seed;
        ResponseFormat = responseFormat;
        ModelId = modelId;
        StopSequences = stopSequences;
        AllowMultipleToolCalls = allowMultipleToolCalls;
        ToolMode = toolMode;
        Tools = tools;
        UpdatedAt = DateTime.UtcNow;
    }

    public override string GetKey()
    {
        return Name;
    }
}
