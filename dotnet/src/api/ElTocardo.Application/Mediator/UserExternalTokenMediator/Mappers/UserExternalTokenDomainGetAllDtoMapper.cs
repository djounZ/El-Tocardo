using ElTocardo.Domain.Mediator.Common.Mappers;
using ElTocardo.Application.Dtos.UserExternalToken;
using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Entities;

namespace ElTocardo.Application.Mediator.UserExternalTokenMediator.Mappers;

public class UserExternalTokenDomainGetAllDtoMapper(UserExternalTokenDomainGetDtoMapper mapper) : AbstractDomainGetAllDtoMapper<UserExternalToken, Guid, UserExternalTokenKey, Dictionary<UserExternalTokenKey, UserExternalTokenItemDto>>
{
    public override Dictionary<UserExternalTokenKey, UserExternalTokenItemDto> MapDomainToDto(IEnumerable<UserExternalToken> tokens)
    {
        return tokens.ToDictionary(
            token => token.GetKey(),
            mapper.MapDomainToDto);
    }
}
