using ElTocardo.Domain.Mediator.Common.Entities;

namespace ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;

public class PresetChatOptions : AbstractEntity<Guid,string>
{
    private PresetChatOptions() { } // EF Core constructor

    public PresetChatOptions(
        string name,
        float? temperature,
        int? maxOutputTokens,
        float? topP,
        int? topK,
        float? frequencyPenalty,
        float? presencePenalty,
        long? seed,
        string? responseFormat,
        string? stopSequences,
        bool? allowMultipleToolCalls,
        string? toolMode,
        string? tools)
    {
        Id = Guid.NewGuid();
        Name = name;
        Temperature = temperature;
        MaxOutputTokens = maxOutputTokens;
        TopP = topP;
        TopK = topK;
        FrequencyPenalty = frequencyPenalty;
        PresencePenalty = presencePenalty;
        Seed = seed;
        ResponseFormat = responseFormat;
        StopSequences = stopSequences;
        AllowMultipleToolCalls = allowMultipleToolCalls;
        ToolMode = toolMode;
        Tools = tools;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        ValidateConfiguration();
    }

    public override Guid Id { get; }
    public string Name { get; private set; } = string.Empty;
    public float? Temperature { get; private set; }
    public int? MaxOutputTokens { get; private set; }
    public float? TopP { get; private set; }
    public int? TopK { get; private set; }
    public float? FrequencyPenalty { get; private set; }
    public float? PresencePenalty { get; private set; }
    public long? Seed { get; private set; }
    public string? ResponseFormat { get; private set; }
    public string? StopSequences { get; private set; }
    public bool? AllowMultipleToolCalls { get; private set; }
    public string? ToolMode { get; private set; }
    public string? Tools { get; private set; }

    public void Update(
        float? temperature,
        int? maxOutputTokens,
        float? topP,
        int? topK,
        float? frequencyPenalty,
        float? presencePenalty,
        long? seed,
        string? responseFormat,
        string? stopSequences,
        bool? allowMultipleToolCalls,
        string? toolMode,
        string? tools)
    {
        Temperature = temperature;
        MaxOutputTokens = maxOutputTokens;
        TopP = topP;
        TopK = topK;
        FrequencyPenalty = frequencyPenalty;
        PresencePenalty = presencePenalty;
        Seed = seed;
        ResponseFormat = responseFormat;
        StopSequences = stopSequences;
        AllowMultipleToolCalls = allowMultipleToolCalls;
        ToolMode = toolMode;
        Tools = tools;
        UpdatedAt = DateTime.UtcNow;
    }

    private void ValidateConfiguration()
    {

        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new ArgumentException("Name cannot be null or empty", nameof(Name));
        }
    }

    public override string GetKey()
    {
        return Name;
    }
}
