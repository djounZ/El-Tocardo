using ElTocardo.Domain.Mediator.Common.Entities;

namespace ElTocardo.Application.Mediator.Common.Mappers;

public abstract class AbstractDomainGetAllDtoMapper<TEntity,TId,TKey, TDtos> where TEntity : IEntity<TId,TKey>
{
    public abstract TDtos MapDomainToDto(IEnumerable<TEntity> configurations);
}
