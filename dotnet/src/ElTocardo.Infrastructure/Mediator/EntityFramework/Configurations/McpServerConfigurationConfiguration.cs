using System.Text.Json;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using ElTocardo.Infrastructure.Mediator.EntityFramework.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElTocardo.Infrastructure.Mediator.EntityFramework.Configurations;

public class McpServerConfigurationConfiguration : IEntityTypeConfiguration<McpServerConfiguration>
{
    public void Configure(EntityTypeBuilder<McpServerConfiguration> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.ServerName)
            .IsUnique();

        builder.Property(e => e.Id)
            .ValueGeneratedNever(); // We set the ID in the domain entity

        builder.Property(e => e.ServerName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.Category)
            .HasMaxLength(100);

        builder.Property(e => e.Command)
            .HasMaxLength(500);

        // Store Arguments as JSON
        builder.Property(e => e.Arguments)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<IList<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
            .Metadata.SetValueComparer(ValueComparers.ListStringComparer);

        // Store EnvironmentVariables as JSON
        builder.Property(e => e.EnvironmentVariables)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<IDictionary<string, string?>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, string?>())
            .Metadata.SetValueComparer(ValueComparers.DictionaryStringNullableStringComparer);


        builder.Property(e => e.Endpoint)
            .HasConversion(
                v => v!.ToString(),
                v => new Uri(v))
            .HasMaxLength(2048);

        builder.Property(e => e.TransportType)
            .HasConversion<int>();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();
    }
}
