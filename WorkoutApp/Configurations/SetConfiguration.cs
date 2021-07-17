using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutApp.Entities;

namespace WorkoutApp.Configurations
{
  public class SetConfiguration : IEntityTypeConfiguration<SetEntity>
  {
    public void Configure(EntityTypeBuilder<SetEntity> builder)
    {
      builder.HasKey(_ => _.Id);

      builder.Property(_ => _.Id)
        .ValueGeneratedOnAdd();
      
      builder.HasOne(_ => _.Exercise)
        .WithMany(_ => _.Sets)
        .HasForeignKey(_ => _.ExerciseId)
        .OnDelete(DeleteBehavior.Restrict);
    }
  }
}