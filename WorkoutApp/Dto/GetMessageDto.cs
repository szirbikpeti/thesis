using System;

namespace WorkoutApp.Dto
{
  public class GetMessageDto
  {
    public int Id { get; set; }

    public string Message { get; set; } = null!;
    
    public DateTimeOffset SentOn { get; set; }

    public GetUserDto SenderUser { get; set; } = null!;
    
    public GetUserDto TriggeredUser { get; set; } = null!;
  }
}