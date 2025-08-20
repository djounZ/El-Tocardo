using ElTocardo.Domain.Mediator.Common.Entities;

namespace ElTocardo.Application.Mediator.Common.Mappers;

public abstract class AbstractDomainUpdateCommandMapper<TEntity, TKey,TUpdateCommand> where TEntity : AbstractEntity<TKey>
{
    public abstract void UpdateFromCommand(TEntity domain, TUpdateCommand command);
}
