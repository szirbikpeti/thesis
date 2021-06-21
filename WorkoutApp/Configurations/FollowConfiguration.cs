using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutApp.Entities;

namespace WorkoutApp.Configurations
{
  public class FollowConfiguration: IEntityTypeConfiguration<FollowEntity>
  {
    public void Configure(EntityTypeBuilder<FollowEntity> builder)
    {
      if (builder is null) {
        throw new ArgumentNullException(nameof(builder));
      }

      builder.ToTable("Follows")
        .HasKey(_ => new {FollowerId = _.FollowerId, FollowedId = _.FollowedId});

      builder.Property(_ => _.FollowerId)
        .ValueGeneratedNever();
      
      builder.Property(_ => _.FollowedId)
        .ValueGeneratedNever();

      builder.HasOne(_ => _.FollowerUser)
        .WithMany(_ => _.FollowedUsers)
        .HasForeignKey(_ => _.FollowerId)
        .OnDelete(DeleteBehavior.Restrict);

      builder.HasOne(_ => _.FollowedUser)
        .WithMany(_ => _.FollowerUsers)
        .HasForeignKey(_ => _.FollowedId)
        .OnDelete(DeleteBehavior.Restrict);
    }
  }
}