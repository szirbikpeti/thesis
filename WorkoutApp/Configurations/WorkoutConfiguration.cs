using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutApp.Entities;

namespace WorkoutApp.Configurations
{
  public class WorkoutConfiguration : IEntityTypeConfiguration<WorkoutEntity>
  {
    public void Configure(EntityTypeBuilder<WorkoutEntity> builder)
    {
      if (builder == null) {
        throw new ArgumentNullException(nameof(builder));
      }

      builder.HasKey(_ => _.Id);

      builder.Property(_ => _.Id)
        .ValueGeneratedOnAdd();
      
      builder.HasOne(_ => _.User)
        .WithMany(_ => _.Workouts)
        .HasForeignKey(_ => _.UserId)
        .OnDelete(DeleteBehavior.Restrict);
    }
  }
}