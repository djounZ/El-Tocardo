using ElTocardo.Domain.Mediator.Common.Repositories;
using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Entities;

namespace ElTocardo.Domain.Mediator.UserExternalTokenMediator.Repositories;

public interface IUserExternalTokenRepository: IEntityRepository<UserExternalToken, Guid, UserExternalTokenKey>;
