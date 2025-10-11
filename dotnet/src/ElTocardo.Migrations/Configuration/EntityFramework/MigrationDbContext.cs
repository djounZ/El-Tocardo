using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;
using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Entities;
using ElTocardo.Infrastructure.EntityFramework.Mediator;
using ElTocardo.ServiceDefaults;
using Microsoft.EntityFrameworkCore;
using AuthorizationDbContext = ElTocardo.Authorization.EntityFramework.Infrastructure.AuthorizationDbContext;

namespace ElTocardo.Migrations.Configuration.EntityFramework;

public class MigrationDbContext(DbContextOptions<MigrationDbContext> options,
    ElTocardoEntityFrameworkModelBuilder entityFrameworkConfigurations) : AuthorizationDbContext(options), IApplicationDbContext
{
    public DbSet<McpServerConfiguration> McpServerConfigurations { get; set; } = null!;
    public DbSet<PresetChatOptions> PresetChatOptions { get; set; } = null!;
    public DbSet<PresetChatInstruction> PresetChatInstructions { get; set; } = null!;
    public DbSet<UserExternalToken> UserExternalTokens { get; set; } = null!;
    public DbSet<PersistentCertificate> PersistentCertificates { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        entityFrameworkConfigurations.OnModelCreating(modelBuilder);
    }

}
