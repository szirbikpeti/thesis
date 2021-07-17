using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutApp.Entities;

namespace WorkoutApp.Configurations
{
  public class LikeConfiguration : IEntityTypeConfiguration<LikeEntity>
  {
    public void Configure(EntityTypeBuilder<LikeEntity> builder)
    {
      if (builder == null) {
        throw new ArgumentNullException(nameof(builder));
      }

      builder.ToTable("Likes")
        .HasKey(_ => new { _.PostId, _.UserId });

      builder.Property(_ => _.PostId)
        .ValueGeneratedNever();

      builder.Property(_ => _.UserId)
        .ValueGeneratedNever();

      builder.HasOne(_ => _.Post)
        .WithMany(_ => _.LikingUsers)
        .HasForeignKey(_ => _.PostId)
        .OnDelete(DeleteBehavior.Restrict);

      builder.HasOne(_ => _.User)
        .WithMany(_ => _.LikedPosts)
        .HasForeignKey(_ => _.UserId)
        .OnDelete(DeleteBehavior.Restrict);
    }
  }
}