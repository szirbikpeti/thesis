using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutApp.Entities;

namespace WorkoutApp.Configurations
{
  public class WorkoutFileRelationConfiguration : 
    IEntityTypeConfiguration<WorkoutFileRelationEntity>
  {
    public void Configure(EntityTypeBuilder<WorkoutFileRelationEntity> builder)
    {
      if (builder == null) {
        throw new ArgumentNullException(nameof(builder));
      }

      builder.ToTable("WorkoutFileRelations")
        .HasKey(_ => new { _.WorkoutId, _.FileId });

      builder.Property(_ => _.WorkoutId)
        .ValueGeneratedNever();

      builder.Property(_ => _.FileId)
        .ValueGeneratedNever();

      builder.HasOne(_ => _.Workout)
        .WithMany(_ => _.FileRelationEntities)
        .HasForeignKey(_ => _.WorkoutId)
        .OnDelete(DeleteBehavior.Restrict);

      builder.HasOne(_ => _.File)
        .WithMany(_ => _.WorkoutRelationEntities)
        .HasForeignKey(_ => _.FileId)
        .OnDelete(DeleteBehavior.Restrict);
    }
  }
}