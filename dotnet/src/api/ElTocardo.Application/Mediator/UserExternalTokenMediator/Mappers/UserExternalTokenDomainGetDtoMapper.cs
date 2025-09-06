using ElTocardo.Application.Dtos.UserExternalToken;
using ElTocardo.Domain.Mediator.Common.Mappers;
using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Entities;

namespace ElTocardo.Application.Mediator.UserExternalTokenMediator.Mappers;

public class UserExternalTokenDomainGetDtoMapper : AbstractDomainGetDtoMapper<UserExternalToken, Guid, UserExternalTokenKey, UserExternalTokenItemDto>
{
    public override UserExternalTokenItemDto MapDomainToDto(UserExternalToken token)
    {
        return new UserExternalTokenItemDto(
            token.UserId,
            token.Provider,
            token.Value,
            token.CreatedAt.UtcDateTime,
            token.UpdatedAt.UtcDateTime);
    }
}
