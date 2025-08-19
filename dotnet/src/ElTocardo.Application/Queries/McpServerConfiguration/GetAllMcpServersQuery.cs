namespace ElTocardo.Application.Queries.McpServerConfiguration;

public record GetAllMcpServersQuery
{
    private GetAllMcpServersQuery()
    {

    }

    public static GetAllMcpServersQuery Instance { get; } = new GetAllMcpServersQuery();
};
