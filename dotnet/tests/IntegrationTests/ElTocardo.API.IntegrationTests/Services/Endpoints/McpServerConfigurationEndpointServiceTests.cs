using ElTocardo.API.IntegrationTests.Configuration;
using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Services;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using ElTocardo.Infrastructure.EntityFramework.Mediator.Common.Data;
using Microsoft.Extensions.DependencyInjection;

namespace ElTocardo.API.IntegrationTests.Services.Endpoints;

[Collection("Global test collection")]
public class McpServerConfigurationEndpointServiceTests : AbstractDbSetServiceIntegrationTests<McpServerConfiguration>
{
    private readonly IMcpServerConfigurationEndpointService _mcpServerConfigurationEndpointService;

    public McpServerConfigurationEndpointServiceTests(GlobalTestFixture fixture) :
        base(fixture.ServiceProvider.GetRequiredService<ApplicationDbContext>(),
            fixture.ServiceProvider.GetRequiredService<ApplicationDbContext>().McpServerConfigurations)
    {
        var serviceProvider = fixture.ServiceProvider;


        _mcpServerConfigurationEndpointService =
            serviceProvider.GetRequiredService<IMcpServerConfigurationEndpointService>();
    }

    [Fact]
    public async Task CreateServer_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var serverDto = new McpServerConfigurationItemDto(
            "test",
            "node",
            new List<string> { "server.js" },
            new Dictionary<string, string?> { { "NODE_ENV", "development" } },
            null);

        // Act
        var result = await _mcpServerConfigurationEndpointService.CreateServerAsync("test-server", serverDto);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task CreateServer_WithDuplicateName_ShouldReturnFailure()
    {
        // Arrange
        var serverDto = new McpServerConfigurationItemDto(
            "test",
            "node",
            new List<string> { "server.js" },
            null,
            null);

        // Act
        await _mcpServerConfigurationEndpointService.CreateServerAsync("duplicate-server", serverDto);
        var result = await _mcpServerConfigurationEndpointService.CreateServerAsync("duplicate-server", serverDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("error", result.ReadError().Message);
    }

    [Fact]
    public async Task GetAllServers_AfterCreatingServers_ShouldReturnAllServers()
    {
        // Arrange
        var serverDto1 = new McpServerConfigurationItemDto(
            "test1",
            "node",
            new List<string> { "server1.js" },
            null,
            null);

        var serverDto2 = new McpServerConfigurationItemDto(
            "test2",
            "python",
            new List<string> { "server2.py" },
            null,
            null);

        // Act
        await _mcpServerConfigurationEndpointService.CreateServerAsync("server1", serverDto1);
        await _mcpServerConfigurationEndpointService.CreateServerAsync("server2", serverDto2);
        var result = await _mcpServerConfigurationEndpointService.GetAllServersAsync();

        // Assert
        Assert.Equal(2, result.ReadValue().Count);
        Assert.True(result.ReadValue().ContainsKey("server1"));
        Assert.True(result.ReadValue().ContainsKey("server2"));
    }

    [Fact]
    public async Task CreateServer_WithHttpTransportAndNoEndpoint_ShouldReturnFailure()
    {
        // Arrange
        var serverDto = new McpServerConfigurationItemDto(
            "test",
            null,
            null,
            null,
            null, // Missing endpoint for HTTP transport
            McpServerTransportTypeDto.Http);

        // Act
        var result = await _mcpServerConfigurationEndpointService.CreateServerAsync("http-server", serverDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Validation failed", result.ReadError().Message);
    }
}
