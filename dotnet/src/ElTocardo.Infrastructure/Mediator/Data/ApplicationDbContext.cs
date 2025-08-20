using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using ElTocardo.Infrastructure.Mediator.Data.ModelBuilderExtensions;
using Microsoft.EntityFrameworkCore;

namespace ElTocardo.Infrastructure.Mediator.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<McpServerConfiguration> McpServerConfigurations { get; set; } = null!;
    public DbSet<ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities.PresetChatOptions> PresetChatOptions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.BuildMcpServerConfiguration();
        modelBuilder.BuildPresetChatOptions();
    }
}
