using System.ComponentModel.DataAnnotations;

namespace WorkoutApp.Dto
{
  public class LikeDto
  {
    [Required]
    public int PostId { get; set; }
    
    [Required]
    public int UserId { get; set; }
  }
}