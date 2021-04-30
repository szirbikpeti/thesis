using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutApp.Entities;

namespace WorkoutApp.Configurations
{
  public class UserUserRelationConfiguration : IEntityTypeConfiguration<UserUserRelationEntity>
  {
    public void Configure(EntityTypeBuilder<UserUserRelationEntity> builder)
    {
      if (builder is null) {
        throw new ArgumentNullException(nameof(builder));
      }

      builder.ToTable("UserUserRelations")
        .HasKey(_ => new {_.RequestingUserId, _.RequestedUserId});

      builder.Property(_ => _.RequestingUserId)
        .ValueGeneratedNever();
      
      builder.Property(_ => _.RequestedUserId)
        .ValueGeneratedNever();

      builder.HasOne(_ => _.RequestingUser)
        .WithMany(_ => _.RequestedUsers)
        .HasForeignKey(_ => _.RequestingUserId)
        .OnDelete(DeleteBehavior.Restrict);

      builder.HasOne(_ => _.RequestedUser)
        .WithMany(_ => _.RequestingUsers)
        .HasForeignKey(_ => _.RequestedUserId)
        .OnDelete(DeleteBehavior.Restrict);
    }
  }
}