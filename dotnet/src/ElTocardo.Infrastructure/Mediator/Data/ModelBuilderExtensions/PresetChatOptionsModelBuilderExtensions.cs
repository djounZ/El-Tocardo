using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElTocardo.Infrastructure.Mediator.Data.ModelBuilderExtensions;

public static class PresetChatOptionsModelBuilderExtensions
{
    public static ModelBuilder BuildPresetChatOptions(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PresetChatOptions>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Instructions).HasMaxLength(-1); // No length restriction
            entity.Property(e => e.Temperature);
            entity.Property(e => e.MaxOutputTokens);
            entity.Property(e => e.TopP);
            entity.Property(e => e.TopK);
            entity.Property(e => e.FrequencyPenalty);
            entity.Property(e => e.PresencePenalty);
            entity.Property(e => e.Seed);
            entity.Property(e => e.ResponseFormat); // JSON
            entity.Property(e => e.StopSequences); // comma-separated
            entity.Property(e => e.AllowMultipleToolCalls);
            entity.Property(e => e.ToolMode).HasMaxLength(500);
            entity.Property(e => e.Tools); // JSON
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
        });
        return modelBuilder;
    }
}
