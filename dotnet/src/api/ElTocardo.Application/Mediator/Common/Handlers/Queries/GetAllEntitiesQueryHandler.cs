using ElTocardo.Domain.Mediator.Common.Entities;
using ElTocardo.Domain.Mediator.Common.Repositories;
using ElTocardo.Domain.Mediator.Interfaces;
using ElTocardo.Domain.Mediator.Mappers;
using ElTocardo.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.Common.Handlers.Queries;

public class GetAllEntitiesQueryHandler<TEntity, TId, TKey, TQuery, TDto>(
    IEntityRepository<TEntity, TId,TKey> repository,
    ILogger<GetAllEntitiesQueryHandler<TEntity,  TId,TKey, TQuery, TDto>> logger,
    AbstractDomainGetAllDtoMapper<TEntity,TId,TKey, TDto> commandMapper): IQueryHandler<TQuery, TDto> where TEntity : IEntity<TId,TKey>
{
    private string EntityName => typeof(TEntity).Name;
    public async Task<Result<TDto>> HandleAsync(
        TQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting all {@Entity}",EntityName);

        var allAsync = await repository.GetAllAsync( cancellationToken);
        if (!allAsync.IsSuccess)
        {
            return allAsync.ReadError();
        }


        var result = commandMapper.MapDomainToDto(allAsync.ReadValue());


        logger.LogInformation("Retrieved all {@Entity}",EntityName);
        return result;
    }

}
