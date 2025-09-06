using ElTocardo.Domain.Mediator.Common.Entities;

namespace ElTocardo.Domain.Mediator.Mappers;

public abstract class AbstractDomainUpdateCommandMapper<TEntity,TId, TKey,TUpdateCommand> where TEntity : IEntity<TId,TKey>
{
    public abstract void UpdateFromCommand(TEntity domain, TUpdateCommand command);
}
