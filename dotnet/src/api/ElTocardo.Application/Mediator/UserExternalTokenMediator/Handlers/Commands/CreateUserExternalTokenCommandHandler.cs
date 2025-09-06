using ElTocardo.Application.Mediator.Common.Handlers.Commands;
using ElTocardo.Application.Mediator.UserExternalTokenMediator.Commands;
using ElTocardo.Application.Mediator.UserExternalTokenMediator.Mappers;
using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Entities;
using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.UserExternalTokenMediator.Handlers.Commands;

public class CreateUserExternalTokenCommandHandler(
    IUserExternalTokenRepository repository,
    ILogger<CreateUserExternalTokenCommandHandler> logger,
    IValidator<CreateUserExternalTokenCommand> validator,
    UserExternalTokenDomainCreateCommandMapper createCommandMapper)
    : CreateEntityCommandHandler<UserExternalToken, Guid, UserExternalTokenKey, CreateUserExternalTokenCommand>(repository, logger, validator, createCommandMapper);
