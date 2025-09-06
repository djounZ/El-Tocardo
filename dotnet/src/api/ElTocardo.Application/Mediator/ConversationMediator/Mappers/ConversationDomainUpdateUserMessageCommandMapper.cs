using ElTocardo.Application.Mediator.ConversationMediator.Commands;
using ElTocardo.Domain.Mediator.Common.Mappers;
using ElTocardo.Domain.Mediator.ConversationMediator.Entities;

namespace ElTocardo.Application.Mediator.ConversationMediator.Mappers;

public class ConversationDomainUpdateUserMessageCommandMapper : AbstractDomainUpdateCommandMapper<Conversation, string, string, UpdateConversationAddNewRoundCommand>
{
    public override void UpdateFromCommand(Conversation domain, UpdateConversationAddNewRoundCommand command)
    {
        domain.AddConversationRound(new ConversationRound(command.UserMessage, command.Options, command.Provider ?? string.Empty));
    }
}
