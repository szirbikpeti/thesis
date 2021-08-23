namespace WorkoutApp.Abstractions
{
  public class Claims
  {
    public const string Type = "role-permissions";
    
    public const string UserManagementPermission = "user.management";
    public const string FeedbackManagementPermission = "feedback.management";
    
    public const string WorkoutListPermission = "workout.list";
    public const string WorkoutAddPermission = "workout.add";
    public const string WorkoutUpdatePermission = "workout.update";
    public const string WorkoutDeletePermission = "workout.delete";
    
    public const string PostListPermission = "post.list";
    public const string PostAddPermission = "post.add";
    public const string PostUpdatePermission = "post.update";
    public const string PostDeletePermission = "post.delete";
    
    public const string CommentAddPermission = "comment.add";
    public const string CommentUpdatePermission = "comment.update";
    public const string CommentDeletePermission = "comment.delete";

    public const string MessageSendPermission = "message.send";
  }
}