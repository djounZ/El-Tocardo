using ElTocardo.Domain.Mediator.Common.Entities;
using Microsoft.Extensions.AI;

namespace ElTocardo.Domain.Mediator.ConversationMediator.Entities;

public interface  IConversation : IEntity<string, string>
{

    public string Title { get; }
    public string? Description { get; }
    public IList<IConversationRound> Rounds { get; }
    public ChatOptions? CurrentOptions { get; }
    public string CurrentProvider { get; }

    public void Update(ChatMessage userMessage, ChatOptions? chatOptions, string? provider);
    public void Update(ChatMessage userMessage, ChatResponse chatResponse);
}
