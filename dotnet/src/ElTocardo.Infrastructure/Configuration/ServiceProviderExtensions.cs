using System.Text.Json;
using AI.GithubCopilot.Configuration;
using ElTocardo.Application.Configuration;
using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Domain.Entities;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.ValueObjects;
using ElTocardo.Infrastructure.Data;
using ElTocardo.Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ElTocardo.Infrastructure.Configuration;

public static class ServiceProviderExtensions
{
    public static async Task UseElTocardoInfrastructureAsync(this IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await serviceProvider.UseAiGithubCopilotAsync(cancellationToken);
        await serviceProvider.UseElTocardoApplicationAsync(cancellationToken);
        await serviceProvider.InitializeDatabaseAsync(cancellationToken);
    }

    private static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
        var infrastructureOptions = scope.ServiceProvider.GetRequiredService<IOptions<ElTocardoInfrastructureOptions>>();

        try
        {
            logger.LogInformation("Ensuring database is created and up to date");
            await context.Database.EnsureCreatedAsync(cancellationToken);

            // Check if we need to migrate data from JSON file
            await MigrateFromJsonFileIfNeededAsync(context, infrastructureOptions.Value, logger, cancellationToken);

            logger.LogInformation("Database initialization completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }

    private static async Task MigrateFromJsonFileIfNeededAsync(
        ApplicationDbContext context,
        ElTocardoInfrastructureOptions options,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        // Only migrate if database is empty and JSON file exists
        var hasExistingData = await context.McpServerConfigurations.AnyAsync(cancellationToken);
        if (hasExistingData)
        {
            logger.LogInformation("Database already contains MCP server configurations, skipping migration");
            return;
        }

        if (string.IsNullOrEmpty(options.McpServerConfigurationFile) || !File.Exists(options.McpServerConfigurationFile))
        {
            logger.LogInformation("No MCP server configuration file found at: {FilePath}, starting with empty database",
                options.McpServerConfigurationFile);
            return;
        }

        logger.LogInformation("Migrating MCP server configurations from JSON file: {FilePath}",
            options.McpServerConfigurationFile);

        try
        {
            await using var fileStream = File.OpenRead(options.McpServerConfigurationFile);
            var jsonConfiguration = await JsonSerializer.DeserializeAsync<McpServerConfigurationDto>(fileStream, cancellationToken: cancellationToken);

            if (jsonConfiguration?.Servers == null || !jsonConfiguration.Servers.Any())
            {
                logger.LogInformation("No servers found in configuration file");
                return;
            }

            var migratedCount = 0;
            foreach (var (serverName, serverConfig) in jsonConfiguration.Servers)
            {
                try
                {
                    var domainEntity = new McpServerConfiguration(
                        serverName,
                        serverConfig.Category,
                        serverConfig.Command,
                        serverConfig.Arguments,
                        serverConfig.EnvironmentVariables,
                        serverConfig.Endpoint,
                        MapTransportType(serverConfig.Type));

                    context.McpServerConfigurations.Add(domainEntity);
                    migratedCount++;

                    logger.LogDebug("Migrated server configuration: {ServerName}", serverName);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to migrate server configuration: {ServerName}", serverName);
                }
            }

            if (migratedCount > 0)
            {
                await context.SaveChangesAsync(cancellationToken);
                logger.LogInformation("Successfully migrated {Count} server configurations from JSON file", migratedCount);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to migrate data from JSON file: {FilePath}", options.McpServerConfigurationFile);
            // Don't throw - we can continue with an empty database
        }
    }

    private static McpServerTransportType MapTransportType(McpServerTransportTypeDto dto)
    {
        return dto switch
        {
            McpServerTransportTypeDto.Stdio => McpServerTransportType.Stdio,
            McpServerTransportTypeDto.Http => McpServerTransportType.Http,
            _ => throw new ArgumentOutOfRangeException(nameof(dto), dto, "Unknown transport type")
        };
    }
}
