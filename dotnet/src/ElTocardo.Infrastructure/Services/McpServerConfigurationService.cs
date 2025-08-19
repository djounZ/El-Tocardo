using ElTocardo.Application.Commands.McpServerConfiguration;
using ElTocardo.Application.Common.Extensions;
using ElTocardo.Application.Common.Interfaces;
using ElTocardo.Application.Common.Models;
using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Queries.McpServerConfiguration;
using ElTocardo.Application.Services;
using ElTocardo.Domain.ValueObjects;

namespace ElTocardo.Infrastructure.Services;

public class McpServerConfigurationService(
    IQueryHandler<GetAllMcpServersQuery, IDictionary<string, McpServerConfigurationItemDto>> getAllQueryHandler,
    IQueryHandler<GetMcpServerByNameQuery, McpServerConfigurationItemDto?> getByNameQueryHandler,
    ICommandHandler<CreateMcpServerCommand, Result<Guid>> createCommandHandler,
    ICommandHandler<UpdateMcpServerCommand, VoidResult> updateCommandHandler,
    ICommandHandler<DeleteMcpServerCommand, VoidResult> deleteCommandHandler)
    : IMcpServerConfigurationService
{

    public async Task<IDictionary<string, McpServerConfigurationItemDto>> GetAllServersAsync(CancellationToken cancellationToken = default)
    {
        return await getAllQueryHandler.HandleAsync(GetAllMcpServersQuery.Instance, cancellationToken);
    }

    public async Task<McpServerConfigurationItemDto?> GetServerAsync(string serverName, CancellationToken cancellationToken = default)
    {
        return await getByNameQueryHandler.HandleAsync(new GetMcpServerByNameQuery(serverName), cancellationToken);
    }

    public async Task<Result<Guid>> CreateServerAsync(string serverName, McpServerConfigurationItemDto item, CancellationToken cancellationToken = default)
    {
        var command = new CreateMcpServerCommand(
            serverName,
            item.Category,
            item.Command,
            item.Arguments,
            item.EnvironmentVariables,
            item.Endpoint,
            MapFromDto(item.Type));

        return await createCommandHandler.HandleAsync(command, cancellationToken);
    }

    public async Task<VoidResult> UpdateServerAsync(string serverName, McpServerConfigurationItemDto item, CancellationToken cancellationToken = default)
    {
        var command = new UpdateMcpServerCommand(
            serverName,
            item.Category,
            item.Command,
            item.Arguments,
            item.EnvironmentVariables,
            item.Endpoint,
            MapFromDto(item.Type));

        return await updateCommandHandler.HandleAsync(command, cancellationToken);
    }

    public async Task<VoidResult> DeleteServerAsync(string serverName, CancellationToken cancellationToken = default)
    {
        var command = new DeleteMcpServerCommand(serverName);
        return await deleteCommandHandler.HandleAsync(command, cancellationToken);
    }

    private static McpServerTransportType MapFromDto(McpServerTransportTypeDto transportType)
    {
        return transportType.ToDomain();
    }
}
