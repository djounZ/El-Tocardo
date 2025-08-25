using Microsoft.Extensions.AI;

namespace ElTocardo.Domain.Mediator.ConversationMediator.Entities;

public class ConversationRound
{

    private ConversationRound() { }

    public ConversationRound(ChatMessage inputMessage, ChatOptions? options, string provider)
    {
        InputMessage = inputMessage;
        Options = options;
        Provider = provider;
    }

    public  DateTimeOffset CreatedAt { get; protected init; } = DateTimeOffset.UtcNow;
    public  DateTimeOffset UpdatedAt { get; protected set; } = DateTimeOffset.UtcNow;
    public ChatMessage InputMessage { get; } = new ChatMessage();
    public ChatOptions? Options { get; }
    public string Provider { get; } = string.Empty;
    public ChatResponse? Response { get; private set; }

    public void Update(ChatResponse chatResponse)
    {
        if (Response != null)
        {
            throw new InvalidOperationException("Response has already been set for this round.");
        }
        Response = chatResponse;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public ChatMessage[] GetAllMessages()
    {
        if (Response == null)
        {
            return [InputMessage];
        }
        return [InputMessage, ..Response.Messages];
    }
}
