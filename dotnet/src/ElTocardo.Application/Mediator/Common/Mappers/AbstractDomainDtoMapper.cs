using ElTocardo.Domain.Mediator.Common.Entities;

namespace ElTocardo.Application.Mediator.Common.Mappers;

public abstract class AbstractDomainDtoMapper<TEntity,TKey, TGetDto, TGetAllDto> where TEntity : AbstractEntity<TKey>
{
    public abstract TGetAllDto MapDomainToDto(IEnumerable<TEntity> configurations);
    public abstract TGetDto MapDomainToDto(TEntity configuration);
}
