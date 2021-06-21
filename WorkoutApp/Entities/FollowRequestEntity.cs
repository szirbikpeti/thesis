namespace WorkoutApp.Entities
{
  public class FollowRequestEntity
  {
    public int SourceId { get; set; }
    
    public int TargetId { get; set; }
    
    public bool IsBlocked { get; set; }

    public UserEntity SourceUser { get; set; } = null!;
    
    public UserEntity TargetUser { get; set; } = null!;
  }
}