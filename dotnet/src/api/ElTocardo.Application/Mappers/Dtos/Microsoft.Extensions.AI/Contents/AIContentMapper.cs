using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Contents;

public class AIContentMapper(
    ILogger<AIContentMapper> logger,
    IDomainEntityMapper<DataContent, DataContentDto> dataContentMapper,
    IDomainEntityMapper<ErrorContent, ErrorContentDto> errorContentMapper,
    IDomainEntityMapper<FunctionCallContent, FunctionCallContentDto> functionCallContentMapper,
    IDomainEntityMapper<FunctionResultContent, FunctionResultContentDto> functionResultContentMapper,
    IDomainEntityMapper<HostedFileContent, HostedFileContentDto> hostedFileContentDtoMapper,
    IDomainEntityMapper<HostedVectorStoreContent, HostedVectorStoreContentDto> hostedVectorStoreContentDtoMapper,
    IDomainEntityMapper<TextContent, TextContentDto> textContentMapper,
    IDomainEntityMapper<TextReasoningContent, TextReasoningContentDto> textReasoningContentMapper,
    IDomainEntityMapper<UriContent, UriContentDto> uriContentMapper,
    IDomainEntityMapper<UsageContent, UsageContentDto> usageContentMapper
    ): IDomainEntityMapper<AIContent, AiContentDto>
{
    public AiContentDto ToApplication(AIContent domainItem)
    {
        switch (domainItem)
        {
            case DataContent dataContent:
                return dataContentMapper.ToApplication(dataContent);
            case ErrorContent errorContent:
                return errorContentMapper.ToApplication(errorContent);
            case FunctionCallContent functionCallContent:
                return functionCallContentMapper.ToApplication(functionCallContent);
            case FunctionResultContent functionResultContent:
                return functionResultContentMapper.ToApplication(functionResultContent);
            case HostedFileContent hostedFileContent:
                return hostedFileContentDtoMapper.ToApplication(hostedFileContent);
            case HostedVectorStoreContent hostedVectorStoreContent:
                return hostedVectorStoreContentDtoMapper.ToApplication(hostedVectorStoreContent);
            case TextContent textContent:
                return textContentMapper.ToApplication(textContent);
            case TextReasoningContent textReasoningContent:
                return textReasoningContentMapper.ToApplication(textReasoningContent);
            case UriContent uriContent:
                return uriContentMapper.ToApplication(uriContent);
            case UsageContent usageContent:
                return usageContentMapper.ToApplication(usageContent);
            default:
                var notSupportedException = new NotSupportedException($"{domainItem.GetType()} is not supported");
                logger.LogError(notSupportedException, "Failed {@Item}", domainItem);
                throw notSupportedException;
        }
    }


    public AIContent ToDomain(AiContentDto applicationItem)
    {
        switch (applicationItem)
        {
            case DataContentDto dataContent:
                return dataContentMapper.ToDomain(dataContent);
            case ErrorContentDto errorContent:
                return errorContentMapper.ToDomain(errorContent);
            case FunctionCallContentDto functionCallContent:
                return functionCallContentMapper.ToDomain(functionCallContent);
            case FunctionResultContentDto functionResultContent:
                return functionResultContentMapper.ToDomain(functionResultContent);
            case HostedFileContentDto hostedFileContent:
                return hostedFileContentDtoMapper.ToDomain(hostedFileContent);
            case HostedVectorStoreContentDto hostedVectorStoreContent:
                return hostedVectorStoreContentDtoMapper.ToDomain(hostedVectorStoreContent);
            case TextContentDto textContent:
                return textContentMapper.ToDomain(textContent);
            case TextReasoningContentDto textReasoningContent:
                return textReasoningContentMapper.ToDomain(textReasoningContent);
            case UriContentDto uriContent:
                return uriContentMapper.ToDomain(uriContent);
            case UsageContentDto usageContent:
                return usageContentMapper.ToDomain(usageContent);
            default:
                var notSupportedException = new NotSupportedException($"{applicationItem.GetType()} is not supported");
                logger.LogError(notSupportedException, "Failed {@Item}", applicationItem);
                throw notSupportedException;
        }
    }
}
