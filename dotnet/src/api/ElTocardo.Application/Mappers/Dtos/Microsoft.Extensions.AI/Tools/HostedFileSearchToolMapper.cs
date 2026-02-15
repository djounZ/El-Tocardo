using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Tools;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Tools;

public class HostedFileSearchToolMapper(
    IDomainEntityMapper<AIContent, AiContentDto> aiContentMapper
) : IDomainEntityMapper<HostedFileSearchTool, HostedFileSearchToolDto>
{
    public HostedFileSearchToolDto ToApplication(HostedFileSearchTool domainItem)
    {
        var name = domainItem.Name;
        var description = domainItem.Description;
        var inputs = domainItem.Inputs?.Select(aiContentMapper.ToApplication).ToList() ?? [];
        var maximumResultCount = domainItem.MaximumResultCount;

        var result = new HostedFileSearchToolDto(
            name,
            description,
            inputs,
            maximumResultCount
        );

        return result;
    }

    public HostedFileSearchTool ToDomain(HostedFileSearchToolDto applicationItem)
    {
        var inputs = applicationItem.Inputs?.Select(aiContentMapper.ToDomain).ToList();
        var maximumResultCount = applicationItem.MaximumResultCount;

        var result = new HostedFileSearchTool()
        {
            Inputs = inputs,
            MaximumResultCount = maximumResultCount
        };

        return result;
    }
}
