using ElTocardo.Domain.Models;

namespace ElTocardo.Domain.Mediator.Interfaces;

public interface ICommandHandler<in TCommand, TResult>
{
    public Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

public interface ICommandHandler<in TCommand>
{
    public Task<VoidResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
