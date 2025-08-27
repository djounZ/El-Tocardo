using ElTocardo.Application.Mediator.Common.Commands;
using ElTocardo.Application.Mediator.Common.Interfaces;
using ElTocardo.Application.Mediator.Common.Mappers;
using ElTocardo.Domain.Mediator.Common.Entities;
using ElTocardo.Domain.Mediator.Common.Repositories;
using ElTocardo.Domain.Models;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.Common.Handlers.Commands;

public class UpdateEntityCommandByKeyHandler<TEntity, TId, TKey, TCommand>(
    IEntityRepository<TEntity,TId, TKey> repository,
    ILogger<UpdateEntityCommandByKeyHandler<TEntity, TId, TKey, TCommand>> logger,
    IValidator<TCommand> validator,
    AbstractDomainUpdateCommandMapper<TEntity,TId, TKey,TCommand> commandMapper)
    : ICommandHandler<TCommand> where TEntity: IEntity<TId,TKey> where TCommand : UpdateCommandBase<TKey>
{

    private string EntityName => typeof(TEntity).Name;
    public async Task<VoidResult> HandleAsync(TCommand command,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating {@Entity}: {ServerName}",EntityName, command);

        // Validate command
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return string.Concat(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        // Get existing configuration
        var byKeyAsync = await repository.GetByKeyAsync(command.Id, cancellationToken);

        if (!byKeyAsync.IsSuccess)
        {
            return byKeyAsync.ReadError();
        }

        var entity = byKeyAsync.ReadValue();

        // Update configuration
        commandMapper.UpdateFromCommand(entity, command);

        // Save changes
        return await repository.UpdateAsync(entity, cancellationToken);
    }
}
