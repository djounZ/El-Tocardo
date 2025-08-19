using ElTocardo.Application.Commands.McpServerConfiguration;
using ElTocardo.Application.Common.Interfaces;
using ElTocardo.Application.Common.Models;
using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Queries.McpServerConfiguration;
using ElTocardo.Application.Services;
using ElTocardo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace ElTocardo.Application.IntegrationTests.McpServerConfiguration;

public class McpServerConfigurationServiceIntegrationTests : IAsyncDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly ApplicationDbContext _dbContext;

    public McpServerConfigurationServiceIntegrationTests( ITestOutputHelper output)
    {
        var services = new ServiceCollection();

        // Add DbContext with in-memory database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}"));

        // Add logging
       //s services.AddLogging(builder => builder.AddConsole());
       services.AddLogging();
        // Register services manually for testing
        services.AddScoped<IMcpServerConfigurationService, ElTocardo.Infrastructure.Services.McpServerConfigurationService>();

        // Add handlers and repositories (simplified for test)
        services.AddScoped<ElTocardo.Domain.Repositories.IMcpServerConfigurationRepository, ElTocardo.Infrastructure.Repositories.McpServerConfigurationRepository>();
        services.AddScoped<ICommandHandler<CreateMcpServerCommand, Result<Guid>>, ElTocardo.Application.Handlers.McpServerConfiguration.CreateMcpServerCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateMcpServerCommand, Result>, ElTocardo.Application.Handlers.McpServerConfiguration.UpdateMcpServerCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteMcpServerCommand, Result>, ElTocardo.Application.Handlers.McpServerConfiguration.DeleteMcpServerCommandHandler>();
        services.AddScoped<IQueryHandler<GetAllMcpServersQuery, IDictionary<string, McpServerConfigurationItemDto>>, ElTocardo.Application.Handlers.McpServerConfiguration.GetAllMcpServersQueryHandler>();
        services.AddScoped<IQueryHandler<GetMcpServerByNameQuery, McpServerConfigurationItemDto?>, ElTocardo.Application.Handlers.McpServerConfiguration.GetMcpServerByNameQueryHandler>();

        // Add validators
        services.AddScoped<FluentValidation.IValidator<CreateMcpServerCommand>, ElTocardo.Application.Validators.McpServerConfiguration.CreateMcpServerCommandValidator>();
        services.AddScoped<FluentValidation.IValidator<UpdateMcpServerCommand>, ElTocardo.Application.Validators.McpServerConfiguration.UpdateMcpServerCommandValidator>();

        _serviceProvider = services.BuildServiceProvider();

         var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
        loggerFactory.AddProvider(new TestOutputLoggerProvider(output));
        _dbContext = _serviceProvider.GetRequiredService<ApplicationDbContext>();

        // Ensure database is created
        _dbContext.Database.EnsureCreated();
    }

    [Fact]
    public async Task CreateServer_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var service = _serviceProvider.GetRequiredService<IMcpServerConfigurationService>();
        var serverDto = new McpServerConfigurationItemDto(
            Category: "test",
            Command: "node",
            Arguments: new List<string> { "server.js" },
            EnvironmentVariables: new Dictionary<string, string?> { { "NODE_ENV", "development" } },
            Endpoint: null,
            Type: McpServerTransportTypeDto.Stdio);

        // Act
        var result = await service.CreateServerAsync("test-server", serverDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
    }

    [Fact]
    public async Task CreateServer_WithDuplicateName_ShouldReturnFailure()
    {
        // Arrange
        var service = _serviceProvider.GetRequiredService<IMcpServerConfigurationService>();
        var serverDto = new McpServerConfigurationItemDto(
            Category: "test",
            Command: "node",
            Arguments: new List<string> { "server.js" },
            EnvironmentVariables: null,
            Endpoint: null,
            Type: McpServerTransportTypeDto.Stdio);

        // Act
        await service.CreateServerAsync("duplicate-server", serverDto);
        var result = await service.CreateServerAsync("duplicate-server", serverDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("already exists", result.Error);
    }

    [Fact]
    public async Task GetAllServers_AfterCreatingServers_ShouldReturnAllServers()
    {
        // Arrange
        var service = _serviceProvider.GetRequiredService<IMcpServerConfigurationService>();
        var serverDto1 = new McpServerConfigurationItemDto(
            Category: "test1",
            Command: "node",
            Arguments: new List<string> { "server1.js" },
            EnvironmentVariables: null,
            Endpoint: null,
            Type: McpServerTransportTypeDto.Stdio);

        var serverDto2 = new McpServerConfigurationItemDto(
            Category: "test2",
            Command: "python",
            Arguments: new List<string> { "server2.py" },
            EnvironmentVariables: null,
            Endpoint: null,
            Type: McpServerTransportTypeDto.Stdio);

        // Act
        await service.CreateServerAsync("server1", serverDto1);
        await service.CreateServerAsync("server2", serverDto2);
        var result = await service.GetAllServersAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.True(result.ContainsKey("server1"));
        Assert.True(result.ContainsKey("server2"));
    }

    [Fact]
    public async Task CreateServer_WithHttpTransportAndNoEndpoint_ShouldReturnFailure()
    {
        // Arrange
        var service = _serviceProvider.GetRequiredService<IMcpServerConfigurationService>();
        var serverDto = new McpServerConfigurationItemDto(
            Category: "test",
            Command: null,
            Arguments: null,
            EnvironmentVariables: null,
            Endpoint: null, // Missing endpoint for HTTP transport
            Type: McpServerTransportTypeDto.Http);

        // Act
        var result = await service.CreateServerAsync("http-server", serverDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Validation failed", result.Error);
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        await _serviceProvider.DisposeAsync();
    }
}

public class TestOutputLoggerProvider(ITestOutputHelper output) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new TestOutputLogger(output, categoryName);
    }

    public void Dispose()
    {
        // No resources to dispose
    }

    private class TestOutputLogger(ITestOutputHelper output, string categoryName) : ILogger
    {
        private static readonly IDisposable DisposableMock = new NoOpDisposable();

        IDisposable ILogger.BeginScope<TState>(TState state)
        {
            return DisposableMock;
        }

        private class NoOpDisposable : IDisposable { public void Dispose() { } }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            try
            {
                output.WriteLine($"[{logLevel}] {categoryName}: {formatter(state, exception)}");
                if (exception != null)
                {
                    output.WriteLine(exception.ToString());
                }
            }
            catch
            {
                /* Ignore output errors */
            }
        }
    }
}
