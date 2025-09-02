using ElTocardo.Domain.Models;

namespace ElTocardo.Domain.Mediator.Interfaces;

public interface IQueryHandler<in TQuery, TResult>
{
    public Task<Result<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}