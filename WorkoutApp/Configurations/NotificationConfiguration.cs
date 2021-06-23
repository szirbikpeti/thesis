using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutApp.Entities;

namespace WorkoutApp.Configurations
{
  public class NotificationConfiguration : IEntityTypeConfiguration<NotificationEntity>
  {
    public void Configure(EntityTypeBuilder<NotificationEntity> builder)
    {
      if (builder == null) {
        throw new ArgumentNullException(nameof(builder));
      }

      builder.HasKey(_ => _.Id);

      builder.Property(_ => _.Id)
        .ValueGeneratedOnAdd();
      
      builder.HasOne(_ => _.SentByUser)
        .WithMany(_ => _.SentNotifications)
        .HasForeignKey(_ => _.SentByUserId)
        .OnDelete(DeleteBehavior.Restrict);

      builder.HasOne(_ => _.ReceivedUser)
        .WithMany(_ => _.ReceivedNotifications)
        .HasForeignKey(_ => _.ReceivedUserId)
        .OnDelete(DeleteBehavior.Restrict);
    }
  }
}