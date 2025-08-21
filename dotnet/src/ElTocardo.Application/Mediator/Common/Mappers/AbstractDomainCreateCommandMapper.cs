using ElTocardo.Domain.Mediator.Common.Entities;

namespace ElTocardo.Application.Mediator.Common.Mappers;

public abstract class AbstractDomainCreateCommandMapper<TEntity, TKey,TCreateCommand> where TEntity : IEntity<TKey>
{
    public abstract TEntity CreateFromCommand(TCreateCommand command);
}
