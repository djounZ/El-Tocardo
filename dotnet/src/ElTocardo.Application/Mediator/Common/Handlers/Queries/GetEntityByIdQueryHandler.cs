using ElTocardo.Application.Mediator.Common.Mappers;
using ElTocardo.Application.Mediator.Common.Queries;
using ElTocardo.Domain.Mediator.Common.Entities;
using ElTocardo.Domain.Mediator.Common.Repositories;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.Common.Handlers.Queries;

public class GetEntityByIdQueryHandler<TEntity, TId, TKey, TQuery, TDto>(
    IEntityRepository<TEntity,TId, TKey> repository,
    ILogger<GetEntityByIdQueryHandler<TEntity, TId, TKey, TQuery, TDto>> logger,
    AbstractDomainGetDtoMapper<TEntity,TId,TKey, TDto> commandMapper): QueryHandlerBase<TQuery, TDto>(logger) where TQuery : GetByIdQueryBase<TId> where TEntity : IEntity<TId,TKey>
{
    private string EntityName => typeof(TEntity).Name;
    protected override async Task<TDto> HandleAsyncImplementation(
        TQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting {@Entity}: {ServerName}",EntityName, query.Id);

        var configuration = await repository.GetByIdAsync(query.Id, cancellationToken);

        var result = commandMapper.MapDomainToDto(configuration!);

        logger.LogInformation("Retrieved {@Entity}: {ServerName}",EntityName,  query.Id);
        return result;
    }

}
