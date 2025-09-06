using ElTocardo.Application.Mediator.UserExternalTokenMediator.Commands;
using ElTocardo.Domain.Mediator.Common.Mappers;
using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Entities;

namespace ElTocardo.Application.Mediator.UserExternalTokenMediator.Mappers;

public class UserExternalTokenDomainCreateCommandMapper : AbstractDomainCreateCommandMapper<UserExternalToken, Guid, UserExternalTokenKey, CreateUserExternalTokenCommand>
{
    public override UserExternalToken CreateFromCommand(CreateUserExternalTokenCommand command)
    {
        return new UserExternalToken(
            command.UserId,
            command.Provider,
            command.Value);
    }
}
