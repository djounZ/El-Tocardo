using ElTocardo.Domain.Mediator.Common.Entities;

namespace ElTocardo.Application.Mediator.Common.Mappers;

public abstract class AbstractDomainGetDtoMapper<TEntity,TKey, TDto> where TEntity : AbstractEntity<TKey>
{
    public abstract TDto MapDomainToDto(TEntity configuration);
}
