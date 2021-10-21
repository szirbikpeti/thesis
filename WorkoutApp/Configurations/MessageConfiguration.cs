using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutApp.Entities;

namespace WorkoutApp.Configurations
{
  public class MessageConfiguration : IEntityTypeConfiguration<MessageEntity>
  {
    public void Configure(EntityTypeBuilder<MessageEntity> builder)
    {
      if (builder == null) {
        throw new ArgumentNullException(nameof(builder));
      }

      builder.HasKey(_ => _.Id);

      builder.Property(_ => _.Id)
        .ValueGeneratedOnAdd();
      
      builder.HasOne(_ => _.SenderUser)
        .WithMany(_ => _.SentMessages)
        .HasForeignKey(_ => _.SenderUserId)
        .OnDelete(DeleteBehavior.NoAction); // TODO - no action

      builder.HasOne(_ => _.TriggeredUser)
        .WithMany(_ => _.ReceivedMessages)
        .HasForeignKey(_ => _.TriggeredUserId)
        .OnDelete(DeleteBehavior.NoAction);
    }
  }
}