using ElTocardo.Domain.Mediator.Common.Entities;
using ElTocardo.Domain.Mediator.Common.Repositories;
using ElTocardo.Domain.Mediator.Interfaces;
using ElTocardo.Domain.Mediator.Mappers;
using ElTocardo.Domain.Mediator.Queries;
using ElTocardo.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.Common.Handlers.Queries;

public class GetEntityByKeyQueryHandler<TEntity, TId, TKey, TQuery, TDto>(
    IEntityRepository<TEntity,TId, TKey> repository,
    ILogger<GetEntityByKeyQueryHandler<TEntity, TId, TKey, TQuery, TDto>> logger,
    AbstractDomainGetDtoMapper<TEntity,TId,TKey, TDto> commandMapper): IQueryHandler<TQuery, TDto> where TQuery : GetByKeyQueryBase<TKey> where TEntity : IEntity<TId,TKey>
{
    private string EntityName => typeof(TEntity).Name;
    public async Task<Result<TDto>> HandleAsync(
        TQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting {@Entity}: {ServerName}",EntityName, query.Key);

        var byKeyAsync = await repository.GetByKeyAsync(query.Key, cancellationToken);

        if (!byKeyAsync.IsSuccess)
        {
            return byKeyAsync.ReadError();
        }

        var result = commandMapper.MapDomainToDto(byKeyAsync.ReadValue());

        logger.LogInformation("Retrieved {@Entity}: {ServerName}",EntityName,  query.Key);
        return result;
    }

}
