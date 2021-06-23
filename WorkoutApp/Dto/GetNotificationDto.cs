using System;
using WorkoutApp.Abstractions;

namespace WorkoutApp.Dto
{
  public class GetNotificationDto
  {
    public int Id { get; set; }

    public DateTimeOffset TriggeredOn { get; set; }

    public DateTimeOffset? DeletedOn { get; set; }

    public NotificationType Type { get; set; }

    public GetUserDto SentByUser { get; set; } = null!;

    public GetUserDto ReceivedUser { get; set; } = null!;
  }
}