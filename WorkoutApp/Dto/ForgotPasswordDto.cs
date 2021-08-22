using System.ComponentModel.DataAnnotations;

namespace WorkoutApp.Dto
{
  public class ForgotPasswordDto
  {
    [Required]
    public string UserName { get; set; } = null!;
  }
}