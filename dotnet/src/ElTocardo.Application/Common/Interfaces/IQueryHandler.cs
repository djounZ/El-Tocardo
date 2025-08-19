namespace ElTocardo.Application.Common.Interfaces;

public interface IQueryHandler<in TQuery, TResult>
{
    public Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
