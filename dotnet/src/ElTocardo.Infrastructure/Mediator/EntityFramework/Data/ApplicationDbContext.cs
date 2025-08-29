using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;
using ElTocardo.Infrastructure.Mediator.EntityFramework.ApplicationUserMediator;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ElTocardo.Infrastructure.Mediator.EntityFramework.Data;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IEntityTypeConfiguration<McpServerConfiguration> mcpServerConfigurationConfiguration,
    IEntityTypeConfiguration<PresetChatInstruction> presetChatInstructionConfiguration,
    IEntityTypeConfiguration<PresetChatOptions> presetChatOptionsConfiguration
    ) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<McpServerConfiguration> McpServerConfigurations { get; set; } = null!;
    public DbSet<PresetChatOptions> PresetChatOptions { get; set; } = null!;
    public DbSet<PresetChatInstruction> PresetChatInstructions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfiguration(mcpServerConfigurationConfiguration);
    modelBuilder.ApplyConfiguration(presetChatInstructionConfiguration);
    modelBuilder.ApplyConfiguration(presetChatOptionsConfiguration);
    }
}
