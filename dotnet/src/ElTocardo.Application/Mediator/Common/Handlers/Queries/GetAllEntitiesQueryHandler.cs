using ElTocardo.Application.Mediator.Common.Mappers;
using ElTocardo.Domain.Mediator.Common.Entities;
using ElTocardo.Domain.Mediator.Common.Repositories;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.Common.Handlers.Queries;

public class GetAllEntitiesQueryHandler<TEntity,  TKey, TQuery, TDto>(
    IEntityRepository<TEntity, TKey> repository,
    ILogger<GetAllEntitiesQueryHandler<TEntity,  TKey, TQuery, TDto>> logger,
    AbstractDomainGetAllDtoMapper<TEntity,TKey, TDto> commandMapper): QueryHandlerBase<TQuery, TDto>(logger) where TEntity : IEntity<TKey>
{
    private string EntityName => typeof(TEntity).Name;
    protected override async Task<TDto> HandleAsyncImplementation(
        TQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting all {@Entity}",EntityName);

        var configuration = await repository.GetAllAsync( cancellationToken);

        var result = commandMapper.MapDomainToDto(configuration!);


        logger.LogInformation("Retrieved all {@Entity}",EntityName);
        return result;
    }

}
