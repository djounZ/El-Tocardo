using ElTocardo.Application.Dtos.Conversation;
using ElTocardo.Domain.Mediator.ConversationMediator.Entities;
using ElTocardo.Domain.Mediator.Mappers;

namespace ElTocardo.Application.Mediator.ConversationMediator.Mappers;

public class ConversationDomainGetAllSummariesDtoMapper(ConversationDomainGetSummaryDtoMapper mapper) : AbstractDomainGetAllDtoMapper<Conversation, string, string, ConversationSummaryDto[]>
{
    public override ConversationSummaryDto[] MapDomainToDto(IEnumerable<Conversation> conversations)
    {
        return [.. conversations.Select(
            mapper.MapDomainToDto)];
    }
}
