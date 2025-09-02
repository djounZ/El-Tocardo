using ElTocardo.Domain.Mediator.Commands;
using ElTocardo.Domain.Mediator.Common.Entities;
using ElTocardo.Domain.Mediator.Common.Repositories;
using ElTocardo.Domain.Mediator.Interfaces;
using ElTocardo.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.Common.Handlers.Commands;

public class DeleteEntityCommandHandler<TEntity, TId, TKey, TCommand>(
    IEntityRepository<TEntity,TId, TKey> repository,
    ILogger<DeleteEntityCommandHandler<TEntity,TId,  TKey, TCommand>> logger)
    : ICommandHandler<TCommand> where TEntity: IEntity<TId,TKey> where TCommand : DeleteCommandBase<TKey>
{

    private string EntityName => typeof(TEntity).Name;
    public async Task<VoidResult> HandleAsync(TCommand command,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting {@Entity}: {ServerName}",EntityName, command.Key);


        // Delete configuration
        return await repository.DeleteAsync(command.Key, cancellationToken);
    }
}
