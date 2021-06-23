using System;
using WorkoutApp.Abstractions;

namespace WorkoutApp.Entities
{
  public class NotificationEntity : 
    IIdentityAwareEntity, 
    IDeleteAwareEntity
  {
    public int Id { get; set; }

    public int SentByUserId { get; set; }

    public int ReceivedUserId { get; set; }

    public DateTimeOffset TriggeredOn { get; set; }

    public DateTimeOffset? DeletedOn { get; set; }

    public NotificationType Type { get; set; }

    public UserEntity SentByUser { get; set; } = null!;

    public UserEntity ReceivedUser { get; set; } = null!;
  }
}