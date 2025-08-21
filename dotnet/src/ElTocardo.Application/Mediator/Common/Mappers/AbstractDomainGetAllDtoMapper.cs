using ElTocardo.Domain.Mediator.Common.Entities;

namespace ElTocardo.Application.Mediator.Common.Mappers;

public abstract class AbstractDomainGetAllDtoMapper<TEntity,TKey, TDtos> where TEntity : IEntity<TKey>
{
    public abstract TDtos MapDomainToDto(IEnumerable<TEntity> configurations);
}
