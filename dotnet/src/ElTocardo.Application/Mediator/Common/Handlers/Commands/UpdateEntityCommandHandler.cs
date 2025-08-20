using ElTocardo.Application.Mediator.Common.Commands;
using ElTocardo.Application.Mediator.Common.Mappers;
using ElTocardo.Domain.Mediator.Common.Entities;
using ElTocardo.Domain.Mediator.Common.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.Common.Handlers.Commands;

public class UpdateEntityCommandHandler<TEntity,  TKey, TCommand>(
    IEntityRepository<TEntity, TKey> repository,
    ILogger<UpdateEntityCommandHandler<TEntity,  TKey, TCommand>> logger,
    IValidator<TCommand> validator,
    AbstractDomainUpdateCommandMapper<TEntity, TKey,TCommand> commandMapper)
    : CommandHandlerBase<TCommand>(logger) where TEntity: AbstractEntity<TKey> where TCommand : UpdateCommandBase<TKey>
{

    private string EntityName => typeof(TEntity).Name;
    protected override async Task HandleAsyncImplementation(TCommand command,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating {@Entity}: {ServerName}",EntityName, command);

        // Validate command
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        // Get existing configuration
        var configuration = await repository.GetByKeyAsync(command.Key, cancellationToken);

        // Update configuration
        commandMapper.UpdateFromCommand(configuration!, command);

        // Save changes
        await repository.UpdateAsync(configuration!, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("{@Entity} updated successfully: {ServerName}",EntityName, command);
    }
}