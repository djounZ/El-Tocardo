namespace ElTocardo.Application.Queries.McpServerConfiguration;

public sealed record GetAllMcpServersQuery
{
    private GetAllMcpServersQuery()
    {

    }

    public static GetAllMcpServersQuery Instance { get; } = new GetAllMcpServersQuery();
};
