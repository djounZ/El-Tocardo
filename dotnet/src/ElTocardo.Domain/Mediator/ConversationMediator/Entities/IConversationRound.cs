using ElTocardo.Domain.Mediator.Common.Entities;
using Microsoft.Extensions.AI;

namespace ElTocardo.Domain.Mediator.ConversationMediator.Entities;

public interface IConversationRound : IEntity<int, int>
{
    public ChatMessage UserMessage  { get; }
    public ChatOptions? Options { get; }
    public string Provider { get; }
    public ChatResponse? Response { get; }

    public void Update(ChatResponse chatResponse);
}
