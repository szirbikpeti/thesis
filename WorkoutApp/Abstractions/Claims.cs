namespace WorkoutApp.Abstractions
{
  public class Claims
  {
    public const string Type = "role-permissions";
    
    public const string UserManagementPermission = "user.management";
    public const string WorkoutManagementPermission = "workout.management";
    public const string PostManagementPermission = "post.management";
    
    public const string PostListPermission = "post.list";
    
    public const string CommentAddPermission = "comment.add";
    
    public const string MessageSendPermission = "message.send";
  }
}