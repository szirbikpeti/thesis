namespace WorkoutApp.Dto
{
  public class UpdatePasswordDto
  {
    public string OldPassword { get; set; } = null!;
    
    public string NewPassword { get; set; } = null!;
  }
}