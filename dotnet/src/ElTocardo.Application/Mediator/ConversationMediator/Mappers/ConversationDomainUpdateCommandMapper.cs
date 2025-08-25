using ElTocardo.Application.Mediator.Common.Mappers;
using ElTocardo.Application.Mediator.ConversationMediator.Commands;
using ElTocardo.Domain.Mediator.ConversationMediator.Entities;

namespace ElTocardo.Application.Mediator.ConversationMediator.Mappers;

public class ConversationDomainUpdateChatResponseCommandMapper : AbstractDomainUpdateCommandMapper<Conversation, string, string, UpdateConversationUpdateRoundCommand>
{
    public override void UpdateFromCommand(Conversation domain, UpdateConversationUpdateRoundCommand command)
    {
        domain.UpdateConversationRound(command.ChatResponse);
    }
}
