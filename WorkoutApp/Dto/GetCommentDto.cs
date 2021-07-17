using System;

namespace WorkoutApp.Dto
{
  public class GetCommentDto
  {
    public int Id { get; set; }

    public string Comment { get; set; } = null!;
    
    public DateTimeOffset CommentedOn { get; set; }
    
    public DateTimeOffset ModifiedOn { get; set; }
  }
}