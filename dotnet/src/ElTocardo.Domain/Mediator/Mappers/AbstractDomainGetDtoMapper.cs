using ElTocardo.Domain.Mediator.Common.Entities;

namespace ElTocardo.Domain.Mediator.Mappers;

public abstract class AbstractDomainGetDtoMapper<TEntity,TId,TKey, TDto> where TEntity : IEntity<TId,TKey>
{
    public abstract TDto MapDomainToDto(TEntity configuration);
}
