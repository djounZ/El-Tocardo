using ElTocardo.Application.Dtos.UserExternalToken;
using ElTocardo.Application.Mediator.Common.Handlers.Queries;
using ElTocardo.Application.Mediator.UserExternalTokenMediator.Mappers;
using ElTocardo.Application.Mediator.UserExternalTokenMediator.Queries;
using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Entities;
using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Repositories;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.UserExternalTokenMediator.Handlers.Queries;

public class GetAllUserExternalTokensQueryHandler(
    IUserExternalTokenRepository repository,
    ILogger<GetAllUserExternalTokensQueryHandler> logger,
    UserExternalTokenDomainGetAllDtoMapper mapper)
    : GetAllEntitiesQueryHandler<UserExternalToken, Guid, UserExternalTokenKey, GetAllUserExternalTokensQuery,
        Dictionary<UserExternalTokenKey, UserExternalTokenItemDto>>(repository, logger, mapper);
