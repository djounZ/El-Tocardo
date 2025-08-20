using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Mediator.Common.Interfaces;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Commands;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Handlers;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Queries;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Validators;
using ElTocardo.Application.Services;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Repositories;
using ElTocardo.Domain.Repositories;
using ElTocardo.Infrastructure.Data;
using ElTocardo.Infrastructure.Repositories;
using ElTocardo.Infrastructure.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace ElTocardo.Application.IntegrationTests.McpServerConfiguration;

public class McpServerConfigurationServiceIntegrationTests : IAsyncDisposable
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ServiceProvider _serviceProvider;

    public McpServerConfigurationServiceIntegrationTests(ITestOutputHelper output)
    {
        var services = new ServiceCollection();

        // Add DbContext with in-memory database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}"));

        // Add logging
        //s services.AddLogging(builder => builder.AddConsole());
        services.AddLogging();
        // Register services manually for testing
        services.AddScoped<IMcpServerConfigurationService, McpServerConfigurationService>();

        // Add handlers and repositories (simplified for test)
        services.AddScoped<IMcpServerConfigurationRepository, McpServerConfigurationRepository>();
        services.AddScoped<ICommandHandler<CreateMcpServerCommand, Guid>, CreateMcpServerCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateMcpServerCommand>, UpdateMcpServerCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteMcpServerCommand>, DeleteMcpServerCommandHandler>();
        services
            .AddScoped<IQueryHandler<GetAllMcpServersQuery, IDictionary<string, McpServerConfigurationItemDto>>,
                GetAllMcpServersQueryHandler>();
        services
            .AddScoped<IQueryHandler<GetMcpServerByNameQuery, McpServerConfigurationItemDto>,
                GetMcpServerByNameQueryHandler>();

        // Add validators
        services.AddScoped<IValidator<CreateMcpServerCommand>, CreateMcpServerCommandValidator>();
        services.AddScoped<IValidator<UpdateMcpServerCommand>, UpdateMcpServerCommandValidator>();

        _serviceProvider = services.BuildServiceProvider();

        var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
        loggerFactory.AddProvider(new TestOutputLoggerProvider(output));
        _dbContext = _serviceProvider.GetRequiredService<ApplicationDbContext>();

        // Ensure database is created
        _dbContext.Database.EnsureCreated();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        await _serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task CreateServer_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var service = _serviceProvider.GetRequiredService<IMcpServerConfigurationService>();
        var serverDto = new McpServerConfigurationItemDto(
            "test",
            "node",
            new List<string> { "server.js" },
            new Dictionary<string, string?> { { "NODE_ENV", "development" } },
            null,
            McpServerTransportTypeDto.Stdio);

        // Act
        var result = await service.CreateServerAsync("test-server", serverDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.ReadValue());
    }

    [Fact]
    public async Task CreateServer_WithDuplicateName_ShouldReturnFailure()
    {
        // Arrange
        var service = _serviceProvider.GetRequiredService<IMcpServerConfigurationService>();
        var serverDto = new McpServerConfigurationItemDto(
            "test",
            "node",
            new List<string> { "server.js" },
            null,
            null,
            McpServerTransportTypeDto.Stdio);

        // Act
        await service.CreateServerAsync("duplicate-server", serverDto);
        var result = await service.CreateServerAsync("duplicate-server", serverDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("already exists", result.ReadError().Message);
    }

    [Fact]
    public async Task GetAllServers_AfterCreatingServers_ShouldReturnAllServers()
    {
        // Arrange
        var service = _serviceProvider.GetRequiredService<IMcpServerConfigurationService>();
        var serverDto1 = new McpServerConfigurationItemDto(
            "test1",
            "node",
            new List<string> { "server1.js" },
            null,
            null,
            McpServerTransportTypeDto.Stdio);

        var serverDto2 = new McpServerConfigurationItemDto(
            "test2",
            "python",
            new List<string> { "server2.py" },
            null,
            null,
            McpServerTransportTypeDto.Stdio);

        // Act
        await service.CreateServerAsync("server1", serverDto1);
        await service.CreateServerAsync("server2", serverDto2);
        var result = await service.GetAllServersAsync();

        // Assert
        Assert.Equal(2, result.ReadValue().Count);
        Assert.True(result.ReadValue().ContainsKey("server1"));
        Assert.True(result.ReadValue().ContainsKey("server2"));
    }

    [Fact]
    public async Task CreateServer_WithHttpTransportAndNoEndpoint_ShouldReturnFailure()
    {
        // Arrange
        var service = _serviceProvider.GetRequiredService<IMcpServerConfigurationService>();
        var serverDto = new McpServerConfigurationItemDto(
            "test",
            null,
            null,
            null,
            null, // Missing endpoint for HTTP transport
            McpServerTransportTypeDto.Http);

        // Act
        var result = await service.CreateServerAsync("http-server", serverDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Validation failed", result.ReadError().Message);
    }
}
