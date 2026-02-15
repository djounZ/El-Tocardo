using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(DataContentDto), "data")]
[JsonDerivedType(typeof(ErrorContentDto), "error")]
[JsonDerivedType(typeof(FunctionCallContentDto), "function_call")]
[JsonDerivedType(typeof(FunctionResultContentDto), "function_result")]
[JsonDerivedType(typeof(HostedFileContentDto), "hosted_file")]
[JsonDerivedType(typeof(HostedVectorStoreContentDto), "hosted_vector_store")]
[JsonDerivedType(typeof(TextContentDto), "text")]
[JsonDerivedType(typeof(TextReasoningContentDto), "reasoning")]
[JsonDerivedType(typeof(UriContentDto), "uri")]
[JsonDerivedType(typeof(UsageContentDto), "usage")]
public abstract record AiContentDto(
    [property: JsonPropertyName("annotations")]
    IList<AiAnnotationDto>? Annotations);
