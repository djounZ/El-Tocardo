namespace ElTocardo.Application.Mediator.McpServerConfigurationMediator.Queries;

public sealed record GetAllMcpServersQuery
{
    private GetAllMcpServersQuery()
    {

    }

    public static GetAllMcpServersQuery Instance { get; } = new GetAllMcpServersQuery();
};
