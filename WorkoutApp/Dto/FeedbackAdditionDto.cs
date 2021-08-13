using System.ComponentModel.DataAnnotations;

namespace WorkoutApp.Dto
{
  public class FeedbackAdditionDto
  {
    [Required]
    public string Feedback { get; set; } = null!;
    
    [Required]
    public short Stars { get; set; }
  }
}