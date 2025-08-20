using ElTocardo.Application.Mediator.Common.Commands;
using ElTocardo.Domain.Mediator.Common.Entities;
using ElTocardo.Domain.Mediator.Common.Repositories;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.Common.Handlers.Commands;

public class DeleteEntityCommandHandler<TEntity,  TKey, TCommand>(
    IEntityRepository<TEntity, TKey> repository,
    ILogger<DeleteEntityCommandHandler<TEntity,  TKey, TCommand>> logger)
    : CommandHandlerBase<TCommand>(logger) where TEntity: AbstractEntity<TKey> where TCommand : DeleteCommandBase<TKey>
{

    private string EntityName => typeof(TEntity).Name;
    protected override async Task HandleAsyncImplementation(TCommand command,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting {@Entity}: {ServerName}",EntityName, command.Key);

        // Get existing configuration
        var configuration = await repository.GetByKeyAsync(command.Key, cancellationToken);
        if (configuration == null)
        {
            logger.LogWarning("MCP server not found: {ServerName}", command.Key);
            return;
        }

        // Delete configuration
        await repository.DeleteAsync(configuration, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("{@Entity} deleted successfully: {ServerName}",EntityName, command.Key);
    }
}
