using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Mediator.Common.Interfaces;
using ElTocardo.Application.Mediator.Common.Models;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Commands;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Queries;
using ElTocardo.Application.Services;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.ValueObjects;

namespace ElTocardo.Infrastructure.Services.Endpoints;

public class McpServerConfigurationEndpointService(
    IQueryHandler<GetAllMcpServersQuery, Dictionary<string, McpServerConfigurationItemDto>> getAllQueryHandler,
    IQueryHandler<GetMcpServerByNameQuery, McpServerConfigurationItemDto> getByNameQueryHandler,
    ICommandHandler<CreateMcpServerCommand, Guid> createCommandHandler,
    ICommandHandler<UpdateMcpServerCommand> updateCommandHandler,
    ICommandHandler<DeleteMcpServerCommand> deleteCommandHandler)
    : IMcpServerConfigurationEndpointService
{
    public async Task<Result<Dictionary<string, McpServerConfigurationItemDto>>> GetAllServersAsync(
        CancellationToken cancellationToken = default)
    {
        return await getAllQueryHandler.HandleAsync(GetAllMcpServersQuery.Instance, cancellationToken);
    }

    public async Task<Result<McpServerConfigurationItemDto>> GetServerAsync(string serverName,
        CancellationToken cancellationToken = default)
    {
        return await getByNameQueryHandler.HandleAsync(new GetMcpServerByNameQuery(serverName), cancellationToken);
    }

    public async Task<Result<Guid>> CreateServerAsync(string serverName, McpServerConfigurationItemDto item,
        CancellationToken cancellationToken = default)
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

    public async Task<VoidResult> UpdateServerAsync(string serverName, McpServerConfigurationItemDto item,
        CancellationToken cancellationToken = default)
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
        return transportType switch
        {
            McpServerTransportTypeDto.Stdio => McpServerTransportType.Stdio,
            McpServerTransportTypeDto.Http => McpServerTransportType.Http,
            _ => throw new ArgumentOutOfRangeException(nameof(transportType), transportType, null)
        };
    }
}
