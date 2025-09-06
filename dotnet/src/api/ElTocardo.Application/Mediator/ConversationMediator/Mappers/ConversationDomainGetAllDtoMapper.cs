using ElTocardo.Application.Dtos.Conversation;
using ElTocardo.Domain.Mediator.ConversationMediator.Entities;
using ElTocardo.Domain.Mediator.Mappers;

namespace ElTocardo.Application.Mediator.ConversationMediator.Mappers;

public class ConversationDomainGetAllDtoMapper(ConversationDomainGetDtoMapper mapper) : AbstractDomainGetAllDtoMapper<Conversation, string, string, ConversationDto[]>
{
    public override ConversationDto[] MapDomainToDto(IEnumerable<Conversation> conversations)
    {
        return [.. conversations.Select(
            mapper.MapDomainToDto)];
    }
}
