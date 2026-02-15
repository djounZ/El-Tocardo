using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Functions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Functions;

public class DelegatingAiFunctionMapper(ILogger<DelegatingAiFunctionDeclarationMapper> logger) : IDomainEntityMapper<DelegatingAIFunction, DelegatingAiFunctionDto>
{
    public DelegatingAiFunctionDto ToApplication(DelegatingAIFunction domainItem)
    {
        var name = domainItem.Name;
        var description = domainItem.Description;
        var schema = domainItem.JsonSchema.ToString();
        var returnJsonSchema = domainItem.ReturnJsonSchema?.ToString();
        var underlyingMethod = domainItem.UnderlyingMethod?.ToString();

        var result = new DelegatingAiFunctionDto(
            name,
            description,
            schema,
            returnJsonSchema,
            underlyingMethod
        );

        return result;
    }

    public DelegatingAIFunction ToDomain(DelegatingAiFunctionDto applicationItem)
    {
        var notSupportedException = new NotSupportedException("This function is not supported.");
        logger.LogError(notSupportedException,"Failed {@Item}", applicationItem);

        throw notSupportedException;

    }
}
