using System;

namespace WorkoutApp.Dto
{
  public class GetFeedbackDto
  {
    public int Id { get; set; }

    public string Feedback { get; set; } = null!;
    
    public short Stars { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public GetUserDto User { get; set; } = null!;
  }
}