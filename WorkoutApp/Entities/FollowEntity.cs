namespace WorkoutApp.Entities
{
  public class FollowEntity
  {
    public int FollowerId { get; set; }
    
    public int FollowedId { get; set; }

    public UserEntity FollowerUser { get; set; } = null!;
    
    public UserEntity FollowedUser { get; set; } = null!;
  }
}