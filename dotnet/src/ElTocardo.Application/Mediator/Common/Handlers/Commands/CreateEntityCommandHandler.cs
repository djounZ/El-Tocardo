using ElTocardo.Application.Mediator.Common.Interfaces;
using ElTocardo.Application.Mediator.Common.Mappers;
using ElTocardo.Domain.Mediator.Common.Entities;
using ElTocardo.Domain.Mediator.Common.Repositories;
using ElTocardo.Domain.Models;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.Common.Handlers.Commands;

public class CreateEntityCommandHandler<TEntity, TId,  TKey, TCommand>(
    IEntityRepository<TEntity,TId, TKey> repository,
    ILogger<CreateEntityCommandHandler<TEntity,  TId, TKey, TCommand>> logger,
    IValidator<TCommand> validator,
    AbstractDomainCreateCommandMapper<TEntity,TId,TKey,TCommand> commandMapper)
    : ICommandHandler<TCommand, TId> where TEntity: IEntity<TId,TKey>
{

    private string EntityName => typeof(TEntity).Name;
    public async Task<Result<TId>> HandleAsync(TCommand command,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating {@Entity}: {ServerName}",EntityName, command);

        // Validate command
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new ValidationException(validationResult.Errors);
        }

        // Create domain entity
        var configuration = commandMapper.CreateFromCommand(command);
        // Add to repository
        return await repository.AddAsync(configuration, cancellationToken);
    }

}
