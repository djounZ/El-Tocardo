using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using ElTocardo.Infrastructure.Mediator.EntityFramework.ApplicationUserMediator;
using ElTocardo.Infrastructure.Mediator.EntityFramework.Data.ModelBuilderExtensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ElTocardo.Infrastructure.Mediator.EntityFramework.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<McpServerConfiguration> McpServerConfigurations { get; set; } = null!;
    public DbSet<ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities.PresetChatOptions> PresetChatOptions { get; set; } = null!;
    public DbSet<ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Entities.PresetChatInstruction> PresetChatInstructions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    base.OnModelCreating(modelBuilder);
    modelBuilder.BuildMcpServerConfiguration();
    modelBuilder.BuildPresetChatOptions();
    modelBuilder.BuildPresetChatInstruction();
    }
}
