using ElTocardo.Application.Mediator.Common.Handlers.Commands;
using ElTocardo.Application.Mediator.UserExternalTokenMediator.Commands;
using ElTocardo.Application.Mediator.UserExternalTokenMediator.Mappers;
using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Entities;
using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.UserExternalTokenMediator.Handlers.Commands;

public class UpdateUserExternalTokenCommandHandler(
    IUserExternalTokenRepository repository,
    ILogger<UpdateUserExternalTokenCommandHandler> logger,
    IValidator<UpdateUserExternalTokenCommand> validator,
    UserExternalTokenDomainUpdateCommandMapper mapper)
    : UpdateEntityCommandHandler<UserExternalToken, Guid, UserExternalTokenKey, UpdateUserExternalTokenCommand>(repository, logger, validator, mapper);
