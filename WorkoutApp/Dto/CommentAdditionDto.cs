using System.ComponentModel.DataAnnotations;

namespace WorkoutApp.Dto
{
  public class CommentAdditionDto
  {
    [Required]
    public string Comment { get; set; } = null!;
  }
}