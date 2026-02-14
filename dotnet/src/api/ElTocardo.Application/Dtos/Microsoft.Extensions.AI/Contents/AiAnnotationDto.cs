using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(CitationAnnotationDto), "citation")]
public abstract record AiAnnotationDto(
    [property: JsonPropertyName("annotated_regions")]
    IList<AnnotatedRegionDto>? AnnotatedRegions);
