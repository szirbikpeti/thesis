using System.ComponentModel.DataAnnotations;

namespace WorkoutApp.Dto
{
  public class MessageAdditionDto
  {
    [Required]
    public int TriggeredUserId { get; set; }
    
    [Required]
    public string Message { get; set; } = null!;
  }
}