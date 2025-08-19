using System;

namespace ElTocardo.Domain.Entities;

public class PresetChatOptions
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // ChatOptionsDto properties as columns
    public string? ConversationId { get; set; }
    public string? Instructions { get; set; } // No length restriction
    public float? Temperature { get; set; }
    public int? MaxOutputTokens { get; set; }
    public float? TopP { get; set; }
    public int? TopK { get; set; }
    public float? FrequencyPenalty { get; set; }
    public float? PresencePenalty { get; set; }
    public long? Seed { get; set; }
    // Store ResponseFormat as JSON
    public string? ResponseFormat { get; set; }
    public string? ModelId { get; set; }
    // Store StopSequences as comma-separated string for simplicity
    public string? StopSequences { get; set; }
    public bool? AllowMultipleToolCalls { get; set; }
    public string? ToolMode { get; set; }
    // Store Tools as JSON
    public string? Tools { get; set; }
}
