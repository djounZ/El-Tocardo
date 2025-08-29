using ElTocardo.Application.Dtos.Conversation;
using ElTocardo.Application.Mediator.Common.Mappers;
using ElTocardo.Domain.Mediator.ConversationMediator.Entities;

namespace ElTocardo.Application.Mediator.ConversationMediator.Mappers;

public class ConversationDomainGetAllSummariesDtoMapper(ConversationDomainGetSummaryDtoMapper mapper) : AbstractDomainGetAllDtoMapper<Conversation, string, string, ConversationSummaryDto[]>
{
    public override ConversationSummaryDto[] MapDomainToDto(IEnumerable<Conversation> conversations)
    {
        return [.. conversations.Select(
            mapper.MapDomainToDto)];
    }
}
