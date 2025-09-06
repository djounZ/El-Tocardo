

using ElTocardo.Application.Dtos.UserExternalToken;
using ElTocardo.Application.Mediator.UserExternalTokenMediator.Commands;
using ElTocardo.Application.Mediator.UserExternalTokenMediator.Queries;
using ElTocardo.Domain.Mediator.Common.Interfaces;
using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Entities;
using ElTocardo.Domain.Models;

namespace ElTocardo.Application.Services;

public class UserExternalTokenEndpointService(
    IQueryHandler<GetAllUserExternalTokensQuery, Dictionary<UserExternalTokenKey, UserExternalTokenItemDto>> getAllQueryHandler,
    IQueryHandler<GetUserExternalTokenByKeyQuery, UserExternalTokenItemDto> getByNameQueryHandler,
    ICommandHandler<CreateUserExternalTokenCommand, Guid> createCommandHandler,
    ICommandHandler<UpdateUserExternalTokenCommand> updateCommandHandler,
    ICommandHandler<DeleteUserExternalTokenCommand> deleteCommandHandler)
    : IUserExternalTokenEndpointService
{
    public async Task<Result<Dictionary<UserExternalTokenKey, UserExternalTokenItemDto>>> GetAllUserExternalTokensAsync(
        CancellationToken cancellationToken = default)
    {
        return await getAllQueryHandler.HandleAsync(GetAllUserExternalTokensQuery.Instance, cancellationToken);
    }

    public async Task<Result<UserExternalTokenItemDto>> GetUserExternalTokenAsync(string userid, string provider,
        CancellationToken cancellationToken = default)
    {
        return await getByNameQueryHandler.HandleAsync(new GetUserExternalTokenByKeyQuery(new (userid, provider)), cancellationToken);
    }

    public async Task<VoidResult> CreateUserExternalTokenAsync(string userId, string provider, string token,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateUserExternalTokenCommand(
            userId,
            provider,
            token);

        return await createCommandHandler.HandleAsync(command, cancellationToken);
    }

    public async Task<VoidResult> UpdateServerAsync(string userId, string provider, string token,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateUserExternalTokenCommand( new (userId, provider),
            token);

        return await updateCommandHandler.HandleAsync(command, cancellationToken);
    }

    public async Task<VoidResult> DeleteServerAsync(string userId, string provider, CancellationToken cancellationToken = default)
    {
        var command = new DeleteUserExternalTokenCommand( new (userId, provider));
        return await deleteCommandHandler.HandleAsync(command, cancellationToken);
    }
}
