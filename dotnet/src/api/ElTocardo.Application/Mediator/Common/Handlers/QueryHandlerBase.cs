using ElTocardo.Domain.Mediator.Interfaces;
using ElTocardo.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.Common.Handlers;

public abstract class QueryHandlerBase<TQuery, TResult>(ILogger<QueryHandlerBase<TQuery, TResult>> logger)
    : IQueryHandler<TQuery, TResult>
{
    public async Task<Result<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            return await HandleAsyncImplementation(query, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to handle query: {QueryType}", typeof(TQuery).Name);
            return ex;
        }
    }

    protected abstract Task<TResult> HandleAsyncImplementation(TQuery query,
        CancellationToken cancellationToken = default);
}
