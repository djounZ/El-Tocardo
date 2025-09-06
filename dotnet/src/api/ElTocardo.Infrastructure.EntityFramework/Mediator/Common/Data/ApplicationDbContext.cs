using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;
using ElTocardo.Infrastructure.EntityFramework.Mediator.ApplicationUserMediator;
using ElTocardo.Infrastructure.EntityFramework.Mediator.Common.Configurations;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ElTocardo.Infrastructure.EntityFramework.Mediator.Common.Data;

public class AuthorizationDbContext(
    DbContextOptions options)  : IdentityDbContext<ApplicationUser>(options), IDataProtectionKeyContext
{

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
}
public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    ElTocardoEntityFrameworkConfigurationEnumerator entityFrameworkConfigurations
) : AuthorizationDbContext(options)
{
    public DbSet<McpServerConfiguration> McpServerConfigurations { get; set; } = null!;
    public DbSet<PresetChatOptions> PresetChatOptions { get; set; } = null!;
    public DbSet<PresetChatInstruction> PresetChatInstructions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        foreach (dynamic entityFrameworkConfiguration in entityFrameworkConfigurations)
        {
            modelBuilder.ApplyConfiguration(entityFrameworkConfiguration);
        }
    }
}
