namespace ElTocardo.Infrastructure.Mediator.EntityFramework.ApplicationUserMediator.Queries;

public sealed record  GetAllUsersQuery
{
    private GetAllUsersQuery()
    {

    }

    public static GetAllUsersQuery Instance { get; } = new GetAllUsersQuery();
};
