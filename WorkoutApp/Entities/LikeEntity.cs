namespace WorkoutApp.Entities
{
  public class LikeEntity
  {
    public int PostId { get; set; }

    public int UserId { get; set; }

    public PostEntity Post { get; set; } = null!;

    public UserEntity User { get; set; } = null!;
  }
}