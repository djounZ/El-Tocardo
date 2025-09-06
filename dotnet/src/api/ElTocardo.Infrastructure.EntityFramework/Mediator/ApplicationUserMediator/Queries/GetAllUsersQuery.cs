namespace ElTocardo.Infrastructure.EntityFramework.Mediator.ApplicationUserMediator.Queries;

public sealed record  GetAllUsersQuery
{
    private GetAllUsersQuery()
    {

    }

    public static GetAllUsersQuery Instance { get; } = new GetAllUsersQuery();
};
