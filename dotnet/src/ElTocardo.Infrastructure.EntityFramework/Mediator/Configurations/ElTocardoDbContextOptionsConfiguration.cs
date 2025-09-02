using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;
using ElTocardo.Infrastructure.EntityFramework.Mediator.McpServerConfigurationMediator;
using ElTocardo.Infrastructure.EntityFramework.Mediator.PresetChatInstructionMediator;
using ElTocardo.Infrastructure.EntityFramework.Mediator.PresetChatOptionsMediator;
using Microsoft.EntityFrameworkCore;

namespace ElTocardo.Infrastructure.EntityFramework.Mediator.Configurations;

public abstract class ElTocardoDbContextOptionsConfiguration : IElTocardoDbContextOptionsConfiguration
{


    public IEntityTypeConfiguration<McpServerConfiguration>  McpServerConfiguration { get;  } = new McpServerConfigurationEntityTypeConfiguration();
    public IEntityTypeConfiguration<PresetChatInstruction>   PresetChatInstruction { get;  } = new PresetChatInstructionEntityTypeConfiguration();
    public IEntityTypeConfiguration<PresetChatOptions>   PresetChatOptions { get;  } = new PresetChatOptionsEntityTypeConfiguration();

    public abstract void Configure(IServiceProvider serviceProvider, DbContextOptionsBuilder optionsBuilder);
}
