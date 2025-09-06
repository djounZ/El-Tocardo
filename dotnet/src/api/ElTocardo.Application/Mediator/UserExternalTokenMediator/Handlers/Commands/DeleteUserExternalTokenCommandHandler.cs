using ElTocardo.Application.Mediator.Common.Handlers.Commands;
using ElTocardo.Application.Mediator.UserExternalTokenMediator.Commands;
using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Entities;
using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Repositories;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.UserExternalTokenMediator.Handlers.Commands;

public class DeleteUserExternalTokenCommandHandler(
    IUserExternalTokenRepository repository,
    ILogger<DeleteUserExternalTokenCommandHandler> logger)
    : DeleteEntityCommandHandler<UserExternalToken, Guid, UserExternalTokenKey, DeleteUserExternalTokenCommand>(repository, logger);
