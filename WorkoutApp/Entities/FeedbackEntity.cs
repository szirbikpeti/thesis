using System;
using WorkoutApp.Abstractions;

namespace WorkoutApp.Entities
{
  public class FeedbackEntity : IIdentityAwareEntity
  {
    public int Id { get; set; }
    
    public int UserId { get; set; }

    public string Feedback { get; set; } = null!;
    
    public short Stars { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public UserEntity User { get; set; } = null!;
  }
}