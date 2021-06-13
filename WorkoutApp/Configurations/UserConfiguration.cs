using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutApp.Entities;

namespace WorkoutApp.Configurations
{
  internal sealed class UserConfiguration : IEntityTypeConfiguration<UserEntity>
  {
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
      builder.ToTable("AspNetUsers");
      
      builder.HasKey(_ => _.Id);
      
      builder.HasOne(_ => _.ProfilePicture)
        .WithOne(_ => _!.ProfilePictureOfUser!)
        .HasForeignKey<UserEntity>(_ => _.ProfilePictureId)
        .OnDelete(DeleteBehavior.Restrict);

      builder.Property(_ => _.Id)
        .ValueGeneratedOnAdd();
    }
  }
}