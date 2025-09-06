using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElTocardo.Infrastructure.EntityFramework.Mediator.PresetChatOptionsMediator;

public class PresetChatOptionsEntityTypeConfiguration : IEntityTypeConfiguration<PresetChatOptions>
{
    public void Configure(EntityTypeBuilder<PresetChatOptions> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.Name).IsUnique();
        builder.Property(e => e.Name).HasMaxLength(255).IsRequired();
        builder.Property(e => e.Temperature);
        builder.Property(e => e.MaxOutputTokens);
        builder.Property(e => e.TopP);
        builder.Property(e => e.TopK);
        builder.Property(e => e.FrequencyPenalty);
        builder.Property(e => e.PresencePenalty);
        builder.Property(e => e.Seed);
        builder.Property(e => e.ResponseFormat); // JSON
        builder.Property(e => e.StopSequences); // comma-separated
        builder.Property(e => e.AllowMultipleToolCalls);
        builder.Property(e => e.ToolMode).HasMaxLength(500);
        builder.Property(e => e.Tools); // JSON
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt).IsRequired();
    }
}
