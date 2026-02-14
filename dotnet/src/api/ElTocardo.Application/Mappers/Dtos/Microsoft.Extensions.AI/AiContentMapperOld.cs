using ElTocardo.Application.Dtos.Microsoft.Extensions.AI;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI;

public sealed class AiContentMapperOld(ILogger<AiContentMapperOld> logger)
{
    public AIContent MapToAiContent(AiContentDto aiContentDto)
    {
        logger.LogTrace("Mapping To AIContent from AiContentDto {@AiContentDto}", aiContentDto);
        return aiContentDto switch
        {
            DataContentDto dataContentDto => MapToDataContent(dataContentDto),
            ErrorContentDto errorContentDto => MapToErrorContent(errorContentDto),
            FunctionCallContentDto functionCallContentDto => MapToFunctionCallContent(functionCallContentDto),
            FunctionResultContentDto functionResultContentDto => MapToFunctionResultContent(functionResultContentDto),
            TextContentDto textContentDto => MapToTextContent(textContentDto),
            TextReasoningContentDto textReasoningContentDto => MapToTextReasoningContent(textReasoningContentDto),
            UriContentDto uriContentDto => MapToUriContent(uriContentDto),
            UsageContentDto usageContentDto => MapToUsageContent(usageContentDto),
            _ => LogAndThrow<AiContentDto, AIContent>(aiContentDto)
        };
    }

    private TOut LogAndThrow<TIn, TOut>(TIn aiContentDto)
    {
        logger.LogError("Unsupported content type: {ContentType} from  {@AiContentDto}", aiContentDto!.GetType().Name,
            aiContentDto);
        throw new NotSupportedException($"Unsupported content type: {aiContentDto.GetType().Name}");
    }

    private UsageContent MapToUsageContent(UsageContentDto usageContentDto)
    {
        return new UsageContent(
            MapToUsageDetails(usageContentDto.DetailsDto)
        );
    }

    private UriContent MapToUriContent(UriContentDto uriContentDto)
    {
        return new UriContent(
            uriContentDto.Uri,
            uriContentDto.MediaType
        );
    }

    private TextReasoningContent MapToTextReasoningContent(TextReasoningContentDto textReasoningContentDto)
    {
        return new TextReasoningContent(
            textReasoningContentDto.Text
        );
    }

    private TextContent MapToTextContent(TextContentDto textContentDto)
    {
        return new TextContent(
            textContentDto.Text
        );
    }

    private FunctionResultContent MapToFunctionResultContent(FunctionResultContentDto functionResultContentDto)
    {
        return new FunctionResultContent(
            functionResultContentDto.CallId,
            functionResultContentDto.Result
        );
    }

    private FunctionCallContent MapToFunctionCallContent(FunctionCallContentDto functionCallContentDto)
    {
        return new FunctionCallContent(
            functionCallContentDto.CallId,
            functionCallContentDto.Name,
            functionCallContentDto.Arguments
        );
    }

    private ErrorContent MapToErrorContent(ErrorContentDto errorContentDto)
    {
        return new ErrorContent(
            errorContentDto.Message
        );
    }

    private DataContent MapToDataContent(DataContentDto dataContentDto)
    {
        return new DataContent(
            dataContentDto.Uri,
            dataContentDto.MediaType
        );
    }

    private UsageDetails MapToUsageDetails(UsageDetailsDto usageDetailsDto)
    {
        var details = new UsageDetails
        {
            InputTokenCount = usageDetailsDto.InputTokenCount,
            OutputTokenCount = usageDetailsDto.OutputTokenCount,
            TotalTokenCount = usageDetailsDto.TotalTokenCount
        };
        if (usageDetailsDto.AdditionalCounts == null)
        {
            return details;
        }

        var additionalCounts = new AdditionalPropertiesDictionary<long>();
        foreach (var kvp in usageDetailsDto.AdditionalCounts)
        {
            additionalCounts[kvp.Key] = kvp.Value;
        }

        details.AdditionalCounts = additionalCounts;
        return details;
    }

    public AiContentDto MapToAiContentDto(AIContent aiContent)
    {
        logger.LogTrace("Mapping To AiContentDto from AIContent {@AIContent}", aiContent);
        return aiContent switch
        {
            DataContent dataContent => MapToDataContentDto(dataContent),
            ErrorContent errorContent => MapToErrorContentDto(errorContent),
            FunctionCallContent functionCallContent => MapToFunctionCallContentDto(functionCallContent),
            FunctionResultContent functionResultContent => MapToFunctionResultContentDto(functionResultContent),
            TextContent textContent => MapToTextContentDto(textContent),
            TextReasoningContent textReasoningContent => MapToTextReasoningContentDto(textReasoningContent),
            UriContent uriContent => MapToUriContentDto(uriContent),
            UsageContent usageContent => MapToUsageContentDto(usageContent),
            _ => LogAndThrow<AIContent, AiContentDto>(aiContent)
        };
    }

    private UsageContentDto MapToUsageContentDto(UsageContent usageContent)
    {
        return new UsageContentDto(
            null, // Annotations mapping not implemented yet
            new UsageDetailsDto(
                usageContent.Details.InputTokenCount,
                usageContent.Details.OutputTokenCount,
                usageContent.Details.TotalTokenCount,
                usageContent.Details.AdditionalCounts != null
                    ? new Dictionary<string, long>(usageContent.Details.AdditionalCounts)
                    : null
            )
        );
    }

    private UriContentDto MapToUriContentDto(UriContent uriContent)
    {
        return new UriContentDto(
            null, // Annotations mapping not implemented yet
            uriContent.Uri,
            uriContent.MediaType
        );
    }

    private TextReasoningContentDto MapToTextReasoningContentDto(TextReasoningContent textReasoningContent)
    {
        return new TextReasoningContentDto(
            null, // Annotations mapping not implemented yet
            textReasoningContent.Text
        );
    }

    private TextContentDto MapToTextContentDto(TextContent textContent)
    {
        return new TextContentDto(
            null, // Annotations mapping not implemented yet
            textContent.Text
        );
    }

    private FunctionResultContentDto MapToFunctionResultContentDto(FunctionResultContent functionResultContent)
    {
        return new FunctionResultContentDto(
            null, // Annotations mapping not implemented yet
            functionResultContent.CallId,
            functionResultContent.Result
        );
    }

    private FunctionCallContentDto MapToFunctionCallContentDto(FunctionCallContent functionCallContent)
    {
        return new FunctionCallContentDto(
            null, // Annotations mapping not implemented yet
            functionCallContent.CallId,
            functionCallContent.Name,
            functionCallContent.Arguments != null
                ? new Dictionary<string, object?>(functionCallContent.Arguments)
                : null
        );
    }

    private ErrorContentDto MapToErrorContentDto(ErrorContent errorContent)
    {
        return new ErrorContentDto(
            null, // Annotations mapping not implemented yet
            errorContent.Message
        );
    }

    private DataContentDto MapToDataContentDto(DataContent dataContent)
    {
        return new DataContentDto(
            null, // Annotations mapping not implemented yet
            new Uri(dataContent.Uri),
            dataContent.MediaType,
            null // Name property not available in DataContent
        );
    }
}
