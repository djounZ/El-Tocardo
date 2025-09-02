using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Repositories;
using ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Repositories;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Repositories;
using ElTocardo.Infrastructure.EntityFramework.Mediator.ApplicationUserMediator;
using ElTocardo.Infrastructure.EntityFramework.Mediator.Common.Configurations;
using ElTocardo.Infrastructure.EntityFramework.Mediator.Common.Data;
using ElTocardo.Infrastructure.EntityFramework.Mediator.McpServerConfigurationMediator;
using ElTocardo.Infrastructure.EntityFramework.Mediator.PresetChatInstructionMediator;
using ElTocardo.Infrastructure.EntityFramework.Mediator.PresetChatOptionsMediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ElTocardo.Infrastructure.EntityFramework.Configuration;

public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Registers  ElTocardo Infrastructure services and options.
    ///     Requires AiGithubCopilotUserProvider to be registered in the service collection.
    ///     Requires IMemoryCache to be registered in the service collection.
    /// </summary>
    public static IServiceCollection AddElTocardoInfrastructureEntityFramework<TApplicationDbContextOptionsConfiguration>(this IServiceCollection services) where TApplicationDbContextOptionsConfiguration : class, IElTocardoDbContextOptionsConfiguration
    {
        return services
            .AddMediator<TApplicationDbContextOptionsConfiguration>();
    }

    private static IServiceCollection AddMediator<TApplicationDbContextOptionsConfiguration>(this IServiceCollection services) where TApplicationDbContextOptionsConfiguration : class, IElTocardoDbContextOptionsConfiguration
    {

        return services
            .AddEntityFramework<TApplicationDbContextOptionsConfiguration>();
    }


    private static IServiceCollection AddEntityFramework<TApplicationDbContextOptionsConfiguration>(this IServiceCollection services) where TApplicationDbContextOptionsConfiguration : class, IElTocardoDbContextOptionsConfiguration
    {

        services
            .AddSingleton<IElTocardoDbContextOptionsConfiguration,
                TApplicationDbContextOptionsConfiguration>()
            .AddSingleton<IDbContextOptionsConfiguration<ApplicationDbContext>>(sc =>
                sc.GetRequiredService<IElTocardoDbContextOptionsConfiguration>())
            .AddSingleton<IElTocardoEntityFrameworkConfiguration>(sc =>
                sc.GetRequiredService<IElTocardoDbContextOptionsConfiguration>());

        services.TryAddSingleton<ElTocardoEntityFrameworkConfigurationEnumerator>();

        services.AddDbContext<ApplicationDbContext>();
        return services
            .AddServices();
    }


    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        return
            services
                .AddUserService()
                .AddMcpServerConfigurationService()
                .AddPresetChatInstructionService()
                .AddPresetChatOptionsService()
            ;
    }

    private static IServiceCollection AddPresetChatInstructionService(this IServiceCollection services)
    {
        services.AddScoped<IPresetChatInstructionRepository, PresetChatInstructionRepository>();
        return services;
    }
    private static IServiceCollection AddPresetChatOptionsService(this IServiceCollection services)
    {
        services.AddScoped<IPresetChatOptionsRepository, PresetChatOptionsRepository>();
        return services;
    }

    private static IServiceCollection AddUserService(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        return services;
    }


    private static IServiceCollection AddMcpServerConfigurationService(this IServiceCollection services)
    {
        services.AddScoped<IMcpServerConfigurationRepository, McpServerConfigurationRepository>();
        return services;
    }
}
