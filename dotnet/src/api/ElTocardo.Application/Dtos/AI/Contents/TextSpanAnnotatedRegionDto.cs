using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.AI.Contents;

public sealed record TextSpanAnnotatedRegionDto(
    [property: JsonPropertyName("start")] int StartIndex,
    [property: JsonPropertyName("end")] int EndIndex) : AnnotatedRegionDto;
