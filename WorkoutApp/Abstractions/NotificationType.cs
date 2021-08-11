namespace WorkoutApp.Abstractions
{
  public enum NotificationType : short
  {
    FollowRequest = 0,
    DeleteFollowRequest = 1,
    DeleteDeclinedFollowRequest = 2,
    FollowBack = 3,
    AcceptFollowRequest = 4,
    DeclineFollowRequest = 5,
    UnFollow = 6,
    AddLike = 10,
  }
}