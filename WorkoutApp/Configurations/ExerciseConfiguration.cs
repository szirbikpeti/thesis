using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutApp.Entities;

namespace WorkoutApp.Configurations
{
  public class ExerciseConfiguration : IEntityTypeConfiguration<ExerciseEntity>
  {
    public void Configure(EntityTypeBuilder<ExerciseEntity> builder)
    {
      builder.HasKey(_ => _.Id);

      builder.Property(_ => _.Id)
        .ValueGeneratedOnAdd();
      
      builder.HasOne(_ => _.Workout)
        .WithMany(_ => _.Exercises)
        .HasForeignKey(_ => _.WorkoutId)
        .OnDelete(DeleteBehavior.Cascade);
    }
  }
}