using ElTocardo.Domain.Mediator.Common.Entities;

namespace ElTocardo.Domain.Mediator.Mappers;

public abstract class AbstractDomainCreateCommandMapper<TEntity,TId,TKey,TCreateCommand> where TEntity : IEntity<TId,TKey>
{
    public abstract TEntity CreateFromCommand(TCreateCommand command);
}
