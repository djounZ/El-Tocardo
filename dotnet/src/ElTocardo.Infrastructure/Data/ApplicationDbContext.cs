using ElTocardo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ElTocardo.Infrastructure.Data.ModelBuilderExtensions;

namespace ElTocardo.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<McpServerConfiguration> McpServerConfigurations { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.BuildMcpServerConfiguration();
    }
}
