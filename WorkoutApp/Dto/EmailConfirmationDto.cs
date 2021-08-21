namespace WorkoutApp.Dto
{
  public class EmailConfirmationDto
  {
    public string UserId { get; set; } = null!;
    
    public string Token { get; set; } = null!;
  }
}