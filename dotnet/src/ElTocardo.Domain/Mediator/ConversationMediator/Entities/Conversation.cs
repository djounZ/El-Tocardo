using ElTocardo.Domain.Mediator.Common.Entities;
using Microsoft.Extensions.AI;

namespace ElTocardo.Domain.Mediator.ConversationMediator.Entities;

public class  Conversation : AbstractEntity<string, string>
{

    private Conversation() { }

    public Conversation(string title , string? description, ChatMessage userMessage, ChatOptions? chatOptions, string? provider)
    {
        Id = Guid.NewGuid().ToString();
        Title = title;
        Description = description;
        Rounds.Add(new ConversationRound(userMessage, chatOptions, provider ?? string.Empty));
        CurrentOptions = chatOptions;
        CurrentProvider = provider ?? string.Empty;
    }

    public override string Id { get; } = string.Empty;
    public string Title { get; private set;} = string.Empty;
    public string? Description { get; private set;}
    public IList<ConversationRound> Rounds { get;  private set;} = [];
    public ChatOptions? CurrentOptions { get; private set; }
    public string CurrentProvider { get; private set; } = string.Empty;

    public void AddConversationRound(ConversationRound round)
    {
        var id = Rounds.Count;

        if(id>0 && Rounds[^1].Response == null)
        {
            throw new InvalidOperationException("The last round has not been completed with a response.");
        }


        Rounds.Add(round);
        if (round.Options != null)
        {
            CurrentOptions = round.Options;
        }
        if (string.IsNullOrEmpty(round.Provider))
        {
            CurrentProvider = round.Provider;
        }
    }

    public void UpdateConversationRound(ChatResponse chatResponse)
    {
        if (Rounds.Count == 0)
        {
            throw new InvalidOperationException("No rounds available to update.");
        }
        var lastRound = Rounds[^1];
        lastRound.Update(chatResponse);
    }
    public override string GetKey()
    {
        return Title;
    }
}
