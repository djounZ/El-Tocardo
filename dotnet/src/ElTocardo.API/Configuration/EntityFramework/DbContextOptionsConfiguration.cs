using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;
using ElTocardo.Infrastructure.EntityFramework.Mediator.Configurations;
using ElTocardo.Infrastructure.EntityFramework.Mediator.McpServerConfigurationMediator;
using ElTocardo.Infrastructure.EntityFramework.Mediator.PresetChatInstructionMediator;
using ElTocardo.Infrastructure.EntityFramework.Mediator.PresetChatOptionsMediator;
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


    public IEntityTypeConfiguration<McpServerConfiguration>  McpServerConfiguration { get;  } = new McpServerConfigurationEntityTypeConfiguration();
    public IEntityTypeConfiguration<PresetChatInstruction>   PresetChatInstruction { get;  } = new PresetChatInstructionEntityTypeConfiguration();
    public IEntityTypeConfiguration<PresetChatOptions>   PresetChatOptions { get;  } = new PresetChatOptionsEntityTypeConfiguration();
}
