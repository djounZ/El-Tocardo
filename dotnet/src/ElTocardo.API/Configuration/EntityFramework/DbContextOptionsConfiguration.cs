using ElTocardo.API.Configuration.EntityFramework.EntityTypeConfiguration;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;
using ElTocardo.Infrastructure.Mediator.EntityFramework.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ElTocardo.API.Configuration.EntityFramework;

public sealed class DbContextOptionsConfiguration : IElTocardoDbContextOptionsConfiguration
{
    public void Configure(IServiceProvider serviceProvider, DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("el-tocardo-db-postgres"));
        optionsBuilder.UseOpenIddict();
    }


    public IEntityTypeConfiguration<McpServerConfiguration>  McpServerConfiguration { get;  } = new McpServerConfigurationConfiguration();
    public IEntityTypeConfiguration<PresetChatInstruction>   PresetChatInstruction { get;  } = new PresetChatInstructionConfiguration();
    public IEntityTypeConfiguration<PresetChatOptions>   PresetChatOptions { get;  } = new PresetChatOptionsConfiguration();
}
