using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Contents;

public class FunctionCallContentMapper(
    IDomainEntityMapper<AIAnnotation, AiAnnotationDto> aiAnnotationMapper
) : IDomainEntityMapper<FunctionCallContent, FunctionCallContentDto>
{
    public FunctionCallContentDto ToApplication(FunctionCallContent domainItem)
    {
        var annotations = domainItem.Annotations?.Select(aiAnnotationMapper.ToApplication).ToList();
        var callId = domainItem.CallId;
        var name = domainItem.Name;
        var arguments = domainItem.Arguments;
        var informationalOnly = domainItem.InformationalOnly;

        var result = new FunctionCallContentDto(
            annotations,
            callId,
            name,
            arguments,
            informationalOnly
        );

        return result;
    }

    public FunctionCallContent ToDomain(FunctionCallContentDto applicationItem)
    {
        var annotations = applicationItem.Annotations?.Select(aiAnnotationMapper.ToDomain).ToList();
        var callId = applicationItem.CallId;
        var name = applicationItem.Name;
        var arguments = applicationItem.Arguments;
        var informationalOnly = applicationItem.InformationalOnly;

        var result = new FunctionCallContent(callId, name, arguments)
        {
            Annotations = annotations,
            InformationalOnly = informationalOnly
        };

        return result;
    }
}
