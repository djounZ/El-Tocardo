using System.Text.Json;
using ElTocardo.API.Options;
using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.ValueObjects;
using ElTocardo.Infrastructure.Mediator.ApplicationUserMediator;
using ElTocardo.Infrastructure.Mediator.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;

namespace ElTocardo.API.Configuration;

public static class FirstTimeInitialization
{
    public static async Task InitializeIfFirstTimeAsync(this WebApplication webApplication, CancellationToken cancellationToken)
    {
        var serviceProvider = webApplication.Services;
        await serviceProvider.InitializeDatabaseAsync(cancellationToken);
    }

    private static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
        var infrastructureOptions = scope.ServiceProvider.GetRequiredService<IOptions<ElTocardoApiOptions>>();

        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        try
        {
            logger.LogInformation("Ensuring database is created and up to date");
            await context.Database.EnsureCreatedAsync(cancellationToken);

            // Check if we need to migrate data from JSON file
            await MigrateFromJsonFileIfNeededAsync(context, infrastructureOptions.Value, logger, cancellationToken);
            await ClientIdAsyn(manager, cancellationToken);

            var roleManager =  scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roles = new[] { "ADMIN", "USER", "MANAGER" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var findByNameAsync = await userManager.FindByNameAsync("test");
            if (findByNameAsync == null)
            {
                var identityResult = await userManager.CreateAsync(new ApplicationUser("test") { Email = "test@test.com",}, password: "Test66*");
                if (!identityResult.Succeeded)
                {
                    throw new ApplicationException(identityResult.Errors.First().Description);
                }

                findByNameAsync = await userManager.FindByNameAsync("test");
                await userManager.AddToRoleAsync(findByNameAsync!, "ADMIN");

            }

            logger.LogInformation("Database initialization completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }

    private static async Task ClientIdAsyn(IOpenIddictApplicationManager manager, CancellationToken cancellationToken)
    {
        if (await manager.FindByClientIdAsync("postman", cancellationToken) == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "postman",
                ClientSecret = "postman-secret",
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.Password,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                    OpenIddictConstants.Permissions.Scopes.Profile
                }
            }, cancellationToken);
        }
    }

    private static async Task MigrateFromJsonFileIfNeededAsync(
        ApplicationDbContext context,
        ElTocardoApiOptions options,
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
