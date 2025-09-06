namespace ElTocardo.Application.Mediator.UserExternalTokenMediator.Queries;

public sealed record GetAllUserExternalTokensQuery
{
    private GetAllUserExternalTokensQuery() { }
    public static GetAllUserExternalTokensQuery Instance { get; } = new GetAllUserExternalTokensQuery();
};
