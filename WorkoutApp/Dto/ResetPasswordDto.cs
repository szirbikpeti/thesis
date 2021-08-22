using System.ComponentModel.DataAnnotations;

namespace WorkoutApp.Dto
{
  public class ResetPasswordDto
  {
    [Required]
    public string UserId { get; set; } = null!;
    
    [Required]
    public string Token { get; set; } = null!;

    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; } = null!;
  }
}