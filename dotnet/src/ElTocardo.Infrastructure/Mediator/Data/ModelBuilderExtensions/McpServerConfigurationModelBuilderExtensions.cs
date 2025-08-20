using System.Text.Json;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElTocardo.Infrastructure.Mediator.Data.ModelBuilderExtensions;

public static  class McpServerConfigurationModelBuilderExtensions
{


    internal static ModelBuilder BuildMcpServerConfiguration(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<McpServerConfiguration>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.ServerName)
                .IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever(); // We set the ID in the domain entity

            entity.Property(e => e.ServerName)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.Category)
                .HasMaxLength(100);

            entity.Property(e => e.Command)
                .HasMaxLength(500);

            // Store Arguments as JSON
            entity.Property(e => e.Arguments)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<IList<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
                .Metadata.SetValueComparer(ValueComparers.ListStringComparer);

            // Store EnvironmentVariables as JSON
            entity.Property(e => e.EnvironmentVariables)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<IDictionary<string, string?>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, string?>())
                .Metadata.SetValueComparer(ValueComparers.DictionaryStringNullableStringComparer);


            entity.Property(e => e.Endpoint)
                .HasConversion(
                    v => v!.ToString(),
                    v => new Uri(v))
                .HasMaxLength(2048);

            entity.Property(e => e.TransportType)
                .HasConversion<int>();

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .IsRequired();
        });

        return modelBuilder;
    }
}
