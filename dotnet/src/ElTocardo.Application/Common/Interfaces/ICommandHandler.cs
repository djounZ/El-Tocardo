namespace ElTocardo.Application.Common.Interfaces;

public interface ICommandHandler<in TCommand, TResult>
{
    public Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

public interface ICommandHandler<in TCommand>
{
    public Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
