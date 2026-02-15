using System.Text.Json.Serialization;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;

namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Tools;

public record HostedFileSearchToolDto(
    string Name,
    string Description,
    [property: JsonPropertyName("inputs")] IList<AiContentDto> Inputs,
    [property: JsonPropertyName("maximum_result_count")] int? MaximumResultCount)
    : AbstractAiToolDto(Name, Description);