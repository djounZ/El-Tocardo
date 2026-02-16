using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.ModelContextProtocol.Core.Protocol;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(TextContentBlockDto), "text")]
[JsonDerivedType(typeof(ImageContentBlockDto), "image")]
[JsonDerivedType(typeof(AudioContentBlockDto), "audio")]
[JsonDerivedType(typeof(EmbeddedResourceBlockDto), "resource")]
[JsonDerivedType(typeof(ResourceLinkBlockDto), "resource_link")]
public abstract record ContentBlockDto(
    [property: JsonPropertyName("annotations")]
    AnnotationsDto? Annotations
);
