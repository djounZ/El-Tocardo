using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Contents;

public class FunctionResultContentMapper(
    IDomainEntityMapper<AIAnnotation, AiAnnotationDto> aiAnnotationMapper
) : IDomainEntityMapper<FunctionResultContent, FunctionResultContentDto>
{
    public FunctionResultContentDto ToApplication(FunctionResultContent domainItem)
    {
        var annotations = domainItem.Annotations?.Select(aiAnnotationMapper.ToApplication).ToList();
        var callId = domainItem.CallId;
        var result = domainItem.Result;

        var resultDto = new FunctionResultContentDto(
            annotations,
            callId,
            result
        );

        return resultDto;
    }

    public FunctionResultContent ToDomain(FunctionResultContentDto applicationItem)
    {
        var annotations = applicationItem.Annotations?.Select(aiAnnotationMapper.ToDomain).ToList();
        var callId = applicationItem.CallId;
        var result = applicationItem.Result;

        var domainResult = new FunctionResultContent(callId, result)
        {
            Annotations = annotations
        };

        return domainResult;
    }
}
