using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;

public sealed record TextSpanAnnotatedRegionDto(
    [property: JsonPropertyName("start")] int StartIndex,
    [property: JsonPropertyName("end")] int EndIndex) : AnnotatedRegionDto;
