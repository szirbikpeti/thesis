using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutApp.Entities;

namespace WorkoutApp.Configurations
{
  public class FollowRequestConfiguration : IEntityTypeConfiguration<FollowRequestEntity>
  {
    public void Configure(EntityTypeBuilder<FollowRequestEntity> builder)
    {
      if (builder is null) {
        throw new ArgumentNullException(nameof(builder));
      }

      builder.ToTable("FollowRequests")
        .HasKey(_ => new {SourceId = _.SourceId, TargetId = _.TargetId});

      builder.Property(_ => _.SourceId)
        .ValueGeneratedNever();
      
      builder.Property(_ => _.TargetId)
        .ValueGeneratedNever();

      builder.HasOne(_ => _.SourceUser)
        .WithMany(_ => _.TargetUsers)
        .HasForeignKey(_ => _.SourceId)
        .OnDelete(DeleteBehavior.Restrict);

      builder.HasOne(_ => _.TargetUser)
        .WithMany(_ => _.SourceUsers)
        .HasForeignKey(_ => _.TargetId)
        .OnDelete(DeleteBehavior.Restrict);
    }
  }
}