using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Repositories;
using ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Repositories;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Repositories;
using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Entities;
using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Repositories;
using ElTocardo.Infrastructure.EntityFramework.Mediator;
using ElTocardo.Infrastructure.EntityFramework.Mediator.McpServerConfigurationMediator;
using ElTocardo.Infrastructure.EntityFramework.Mediator.PresetChatInstructionMediator;
using ElTocardo.Infrastructure.EntityFramework.Mediator.PresetChatOptionsMediator;
using ElTocardo.Infrastructure.EntityFramework.Mediator.UserExternalTokenMediator;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
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
    public static IServiceCollection AddElTocardoInfrastructureEntityFramework<TApplicationDbContextOptionsConfiguration, TApplicationDbContext>(this IServiceCollection services, string applicationName) where TApplicationDbContextOptionsConfiguration : class,  IDbContextOptionsConfiguration<TApplicationDbContext> where TApplicationDbContext : DbContext, IApplicationDbContext
    {
        services.AddDataProtection()
            .PersistKeysToDbContext<TApplicationDbContext>()
            .SetApplicationName(applicationName);
        return services
            .AddMediator<TApplicationDbContextOptionsConfiguration, TApplicationDbContext>();
    }

    private static IServiceCollection AddMediator<TApplicationDbContextOptionsConfiguration, TApplicationDbContext>(this IServiceCollection services) where TApplicationDbContextOptionsConfiguration : class,  IDbContextOptionsConfiguration<TApplicationDbContext> where TApplicationDbContext : DbContext, IApplicationDbContext
    {

        return services
            .AddEntityFramework<TApplicationDbContextOptionsConfiguration, TApplicationDbContext>();
    }


    private static IServiceCollection AddEntityFramework<TApplicationDbContextOptionsConfiguration, TApplicationDbContext>(this IServiceCollection services) where TApplicationDbContextOptionsConfiguration : class,  IDbContextOptionsConfiguration<TApplicationDbContext> where TApplicationDbContext : DbContext, IApplicationDbContext
    {

        services
            .TryAddSingleton<IDbContextOptionsConfiguration<TApplicationDbContext>,
                TApplicationDbContextOptionsConfiguration>();

        services.TryAddSingleton<ElTocardoEntityFrameworkModelBuilder>();

        services.AddDbContext<TApplicationDbContext>();
        services.AddTransient<DbContext>( sc=> sc.GetRequiredService<TApplicationDbContext>());
        services.AddTransient<IApplicationDbContext>( sc=> sc.GetRequiredService<TApplicationDbContext>());
        return services
            .AddServices();
    }


    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        return
            services
                .AddMcpServerConfigurationService()
                .AddPresetChatInstructionService()
                .AddPresetChatOptionsService()
                .AddUserExternalTokenService()
            ;
    }

    private static IServiceCollection AddUserExternalTokenService(this IServiceCollection services)
    {
        services.AddSingleton<UserExternalTokenProtector>();
        services.AddTransient<DbSet<UserExternalToken>>( sc=> sc.GetRequiredService<IApplicationDbContext>().UserExternalTokens);
        services.AddScoped<IUserExternalTokenRepository, UserExternalTokenRepository>();
        return services;
    }

    private static IServiceCollection AddPresetChatInstructionService(this IServiceCollection services)
    {

        services.AddTransient<DbSet<PresetChatInstruction>>( sc=> sc.GetRequiredService<IApplicationDbContext>().PresetChatInstructions);
        services.AddScoped<IPresetChatInstructionRepository, PresetChatInstructionRepository>();
        return services;
    }
    private static IServiceCollection AddPresetChatOptionsService(this IServiceCollection services)
    {
        services.AddTransient<DbSet<PresetChatOptions>>( sc=> sc.GetRequiredService<IApplicationDbContext>().PresetChatOptions);
        services.AddScoped<IPresetChatOptionsRepository, PresetChatOptionsRepository>();
        return services;
    }



    private static IServiceCollection AddMcpServerConfigurationService(this IServiceCollection services)
    {
        services.AddTransient<DbSet<McpServerConfiguration>>( sc=> sc.GetRequiredService<IApplicationDbContext>().McpServerConfigurations);
        services.AddScoped<IMcpServerConfigurationRepository, McpServerConfigurationRepository>();
        return services;
    }
}
