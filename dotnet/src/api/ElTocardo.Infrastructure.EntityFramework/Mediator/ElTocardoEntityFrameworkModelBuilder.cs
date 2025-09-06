using ElTocardo.Infrastructure.EntityFramework.Mediator.McpServerConfigurationMediator;
using ElTocardo.Infrastructure.EntityFramework.Mediator.PresetChatInstructionMediator;
using ElTocardo.Infrastructure.EntityFramework.Mediator.PresetChatOptionsMediator;
using ElTocardo.Infrastructure.EntityFramework.Mediator.UserExternalTokenMediator;
using Microsoft.EntityFrameworkCore;

namespace ElTocardo.Infrastructure.EntityFramework.Mediator;

public class ElTocardoEntityFrameworkModelBuilder
{

    public void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.ApplyConfiguration(new McpServerConfigurationEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PresetChatInstructionEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PresetChatOptionsEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new UserExternalTokenEntityTypeConfiguration());
    }
}
