using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.AI.Contents;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(TextSpanAnnotatedRegionDto), "textSpan")]
public abstract record AnnotatedRegionDto;
