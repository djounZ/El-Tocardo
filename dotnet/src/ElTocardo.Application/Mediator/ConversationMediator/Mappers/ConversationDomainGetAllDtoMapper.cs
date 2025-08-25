using ElTocardo.Application.Dtos.Conversation;
using ElTocardo.Application.Mediator.Common.Mappers;
using ElTocardo.Domain.Mediator.ConversationMediator.Entities;

namespace ElTocardo.Application.Mediator.ConversationMediator.Mappers;

public class ConversationDomainGetAllDtoMapper(ConversationDomainGetDtoMapper mapper) : AbstractDomainGetAllDtoMapper<Conversation, string, string, Dictionary<string, ConversationResponseDto>>
{
    public override Dictionary<string, ConversationResponseDto> MapDomainToDto(IEnumerable<Conversation> conversations)
    {
        return conversations.ToDictionary(
            conversation => conversation.Title,
            mapper.MapDomainToDto);
    }
}
