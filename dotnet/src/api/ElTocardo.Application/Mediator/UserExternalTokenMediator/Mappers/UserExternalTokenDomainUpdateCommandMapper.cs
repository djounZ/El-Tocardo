using ElTocardo.Application.Mediator.UserExternalTokenMediator.Commands;
using ElTocardo.Domain.Mediator.Common.Mappers;
using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Entities;

namespace ElTocardo.Application.Mediator.UserExternalTokenMediator.Mappers;

public class UserExternalTokenDomainUpdateCommandMapper : AbstractDomainUpdateCommandMapper<UserExternalToken, Guid, UserExternalTokenKey, UpdateUserExternalTokenCommand>
{
    public override void UpdateFromCommand(UserExternalToken domain, UpdateUserExternalTokenCommand command)
    {
        domain.Update(command.Value);
    }
}
