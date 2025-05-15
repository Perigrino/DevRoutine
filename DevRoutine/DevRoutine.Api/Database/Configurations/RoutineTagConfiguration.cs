using DevRoutine.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevRoutine.Api.Database.Configurations;

public class RoutineTagConfiguration : IEntityTypeConfiguration<RoutineTag>
{
    public void Configure(EntityTypeBuilder<RoutineTag> builder)
    {
        builder.HasKey(rt => new { rt.RoutineId, rt.TagId });

        // Already applied by the FK definition (Routine, Tag)
        builder.Property(r => r.RoutineId).HasMaxLength(500);
        builder.Property(r => r.TagId).HasMaxLength(500);

        builder.HasOne<Tag>()
        .WithMany()
        .HasForeignKey(rt => rt.TagId);

        builder.HasOne<Routine>()
        .WithMany(r => r.RoutineTags)
        .HasForeignKey(rt => rt.RoutineId);
    }
}
