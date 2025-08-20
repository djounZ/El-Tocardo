using ElTocardo.Application.Mediator.Common.Mappers;
using ElTocardo.Domain.Mediator.Common.Entities;
using ElTocardo.Domain.Mediator.Common.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.Common.Handlers.Commands;

public class CreateEntityCommandHandler<TEntity,  TKey, TCommand>(
    IEntityRepository<TEntity, TKey> repository,
    ILogger<CreateEntityCommandHandler<TEntity,  TKey, TCommand>> logger,
    IValidator<TCommand> validator,
    AbstractDomainCreateCommandMapper<TEntity, TKey,TCommand> commandMapper)
    : CommandHandlerBase<TCommand, Guid>(logger) where TEntity: AbstractEntity<TKey>
{

    private string EntityName => typeof(TEntity).Name;
    protected override async Task<Guid> HandleAsyncImplementation(TCommand command,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating {@Entity}: {ServerName}",EntityName, command);

        // Validate command
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        // Create domain entity
        var configuration = commandMapper.CreateFromCommand(command);

        if (await repository.ExistsAsync(configuration.GetKey(), cancellationToken))
        {
            throw new InvalidOperationException(
                $"{EntityName} with key {configuration.GetKey()} already exists.");
        }
        // Add to repository
        await repository.AddAsync(configuration, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("{@Entity} created successfully: {ServerName} with ID: {Id}",EntityName,
            command, configuration.Id);

        return configuration.Id;
    }

}
