using ElTocardo.Domain.Models;

namespace ElTocardo.Domain.Mediator.Common.Interfaces;

public interface IQueryHandler<in TQuery, TResult>
{
    public Task<Result<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}