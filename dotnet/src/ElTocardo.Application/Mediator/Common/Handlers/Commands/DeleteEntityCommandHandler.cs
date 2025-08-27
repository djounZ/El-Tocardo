using ElTocardo.Application.Mediator.Common.Commands;
using ElTocardo.Domain.Mediator.Common.Entities;
using ElTocardo.Domain.Mediator.Common.Repositories;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.Common.Handlers.Commands;

public class DeleteEntityCommandHandler<TEntity, TId, TKey, TCommand>(
    IEntityRepository<TEntity,TId, TKey> repository,
    ILogger<DeleteEntityCommandHandler<TEntity,TId,  TKey, TCommand>> logger)
    : CommandHandlerBase<TCommand>(logger) where TEntity: IEntity<TId,TKey> where TCommand : DeleteCommandBase<TKey>
{

    private string EntityName => typeof(TEntity).Name;
    protected override async Task HandleAsyncImplementation(TCommand command,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting {@Entity}: {ServerName}",EntityName, command.Key);


        // Delete configuration
        await repository.DeleteAsync(command.Key, cancellationToken);

        logger.LogInformation("{@Entity} deleted successfully: {ServerName}",EntityName, command.Key);
    }
}
