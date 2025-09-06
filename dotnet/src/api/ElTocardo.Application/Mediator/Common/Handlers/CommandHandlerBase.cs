using ElTocardo.Domain.Mediator.Common.Interfaces;
using ElTocardo.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.Common.Handlers;

public abstract class CommandHandlerBase<TCommand>(ILogger<CommandHandlerBase<TCommand>> logger)
    : ICommandHandler<TCommand>
{
    public async Task<VoidResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            await HandleAsyncImplementation(command, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to handle command: {CommandType}", typeof(TCommand).Name);
            return ex;
        }

        return VoidResult.Success;
    }

    protected abstract Task HandleAsyncImplementation(TCommand command, CancellationToken cancellationToken = default);
}
public abstract class CommandHandlerBase<TCommand, TResult>(ILogger<CommandHandlerBase<TCommand, TResult>> logger)
    : ICommandHandler<TCommand, TResult>
{
    public async Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            return await HandleAsyncImplementation(command, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to handle command: {CommandType}", typeof(TCommand).Name);
            return ex;
        }
    }

    protected abstract Task<TResult> HandleAsyncImplementation(TCommand command,
        CancellationToken cancellationToken = default);
}
