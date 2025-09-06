using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElTocardo.Infrastructure.EntityFramework.Mediator.UserExternalTokenMediator;

public class UserExternalTokenEntityTypeConfiguration : IEntityTypeConfiguration<UserExternalToken>
{
    public void Configure(EntityTypeBuilder<UserExternalToken> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => new { e.UserId, e.Provider })
            .IsUnique();

        builder.Property(e => e.Id)
            .ValueGeneratedNever(); // Set in domain entity

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(e => e.Provider)
            .IsRequired();

        builder.Property(e => e.Value)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();
    }
}
