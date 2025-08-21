using ElTocardo.Application.Mediator.Common.Handlers.Commands;
using ElTocardo.Application.Mediator.Common.Mappers;
using ElTocardo.Application.Mediator.Common.Queries;
using ElTocardo.Domain.Mediator.Common.Entities;
using ElTocardo.Domain.Mediator.Common.Repositories;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.Common.Handlers.Queries;

public class GetEntityByKeyQueryHandler<TEntity,  TKey, TQuery, TDto>(
    IEntityRepository<TEntity, TKey> repository,
    ILogger<GetEntityByKeyQueryHandler<TEntity,  TKey, TQuery, TDto>> logger,
    AbstractDomainGetDtoMapper<TEntity,TKey, TDto> commandMapper): QueryHandlerBase<TQuery, TDto>(logger) where TQuery : GetByKeyQueryBase<TKey> where TEntity : IEntity<TKey>
{
    private string EntityName => typeof(TEntity).Name;
    protected override async Task<TDto> HandleAsyncImplementation(
        TQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting {@Entity}: {ServerName}",EntityName, query.Key);

        var configuration = await repository.GetByKeyAsync(query.Key, cancellationToken);

        var result = commandMapper.MapDomainToDto(configuration!);

        logger.LogInformation("Retrieved {@Entity}: {ServerName}",EntityName,  query.Key);
        return result;
    }

}
