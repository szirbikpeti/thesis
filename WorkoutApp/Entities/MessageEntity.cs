using System;
using WorkoutApp.Abstractions;

namespace WorkoutApp.Entities
{
  public class MessageEntity : IIdentityAwareEntity
  {
    public int Id { get; set; }

    public int SenderUserId { get; set; }
    
    public int TriggeredUserId { get; set; }
    
    public string Message { get; set; } = null!;
    
    public DateTimeOffset SentOn { get; set; }

    public UserEntity SenderUser { get; set; } = null!;
    
    public UserEntity TriggeredUser { get; set; } = null!;
  }
}