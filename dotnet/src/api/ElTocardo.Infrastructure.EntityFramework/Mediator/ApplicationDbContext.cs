using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;
using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Entities;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ElTocardo.Infrastructure.EntityFramework.Mediator;

public interface IApplicationDbContext : IDataProtectionKeyContext
{
   public DbSet<McpServerConfiguration> McpServerConfigurations { get; set; }
   public DbSet<PresetChatOptions> PresetChatOptions { get; set; }
   public DbSet<PresetChatInstruction> PresetChatInstructions { get; set; }
   public DbSet<UserExternalToken> UserExternalTokens { get; set; }
}

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    ElTocardoEntityFrameworkModelBuilder entityFrameworkConfigurations
) : DbContext(options), IApplicationDbContext
{
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    public DbSet<McpServerConfiguration> McpServerConfigurations { get; set; } = null!;
    public DbSet<PresetChatOptions> PresetChatOptions { get; set; } = null!;
    public DbSet<PresetChatInstruction> PresetChatInstructions { get; set; } = null!;
    public DbSet<UserExternalToken> UserExternalTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        entityFrameworkConfigurations.OnModelCreating(modelBuilder);
    }
}
