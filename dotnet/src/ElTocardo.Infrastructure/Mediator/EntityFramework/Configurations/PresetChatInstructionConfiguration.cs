using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Entities;

namespace ElTocardo.Infrastructure.Mediator.EntityFramework.Configurations;

public class PresetChatInstructionConfiguration : IEntityTypeConfiguration<PresetChatInstruction>
{
    public void Configure(EntityTypeBuilder<PresetChatInstruction> builder)
    {
        builder.HasKey(x => x.Id);


        builder.HasIndex(e => e.Name)
            .IsUnique();
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Description);
        builder.Property(x => x.ContentType).IsRequired();
        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.UpdatedAt);
    }
}
