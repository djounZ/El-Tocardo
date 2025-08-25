using ElTocardo.Application.Mediator.Common.Mappers;
using ElTocardo.Application.Mediator.ConversationMediator.Commands;
using ElTocardo.Domain.Mediator.ConversationMediator.Entities;

namespace ElTocardo.Application.Mediator.ConversationMediator.Mappers;

public class ConversationDomainCreateCommandMapper : AbstractDomainCreateCommandMapper<Conversation, string, string, CreateConversationCommand>
{
    public override Conversation CreateFromCommand(CreateConversationCommand command)
    {
        return new Conversation(
            command.Title,
            command.Description,
            command.UserMessage,
            command.ChatOptions,
            command.Provider);
    }
}
