namespace ElTocardo.Application.Mediator.ConversationMediator.Queries;

public sealed record GetAllConversationsQuery
{
    private GetAllConversationsQuery()
    {

    }

    public static GetAllConversationsQuery Instance { get; } = new GetAllConversationsQuery();
};
