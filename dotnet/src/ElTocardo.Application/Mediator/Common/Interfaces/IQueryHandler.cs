using ElTocardo.Domain.Models;

namespace ElTocardo.Application.Mediator.Common.Interfaces;

public interface IQueryHandler<in TQuery, TResult>
{
    public Task<Result<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}