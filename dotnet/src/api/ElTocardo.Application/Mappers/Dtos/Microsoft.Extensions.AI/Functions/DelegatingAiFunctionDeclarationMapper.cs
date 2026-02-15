using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Functions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Functions;

public class DelegatingAiFunctionDeclarationMapper(ILogger<DelegatingAiFunctionDeclarationMapper> logger) : IDomainEntityMapper<AIFunctionDeclaration, DelegatingAiFunctionDeclarationDto>
{
    public DelegatingAiFunctionDeclarationDto ToApplication(AIFunctionDeclaration domainItem)
    {
        var name = domainItem.Name;
        var description = domainItem.Description;
        var schema = domainItem.JsonSchema.ToString();
        var returnJsonSchema = domainItem.ReturnJsonSchema.ToString();

        var result = new DelegatingAiFunctionDeclarationDto(
            name,
            description,
            schema,
            returnJsonSchema
        );

        return result;
    }

    public AIFunctionDeclaration ToDomain(DelegatingAiFunctionDeclarationDto applicationItem)
    {
        var notSupportedException = new NotSupportedException("This function is not supported.");
        logger.LogError(notSupportedException,"Failed {@Item}", applicationItem);

        throw notSupportedException;
    }
}
