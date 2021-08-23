using System.Collections.Generic;

namespace WorkoutApp.Abstractions
{
  public class Roles
  {
    public const string Administrator = "Administrator";
    public const string WorkoutManager = "WorkoutManager";
    public const string PostManager = "PostManager";
    public const string CommentManager = "CommentManager";
    public const string MessageSender = "MessageSender";

    public static Dictionary<string, List<string>> GetRolePermissions() =>
      new() {
        {
          Administrator, 
          new List<string> {
            Claims.UserManagementPermission,
            Claims.FeedbackManagementPermission,
            Claims.PostListPermission,
            Claims.PostDeletePermission,
            Claims.CommentAddPermission,
            Claims.CommentUpdatePermission,
            Claims.CommentDeletePermission,
          }
        },
        {
          WorkoutManager, 
          new List<string> {
            Claims.WorkoutListPermission,
            Claims.WorkoutAddPermission,
            Claims.WorkoutUpdatePermission,
            Claims.WorkoutDeletePermission,
          }
        },
        {
          PostManager, 
          new List<string> {
            Claims.PostListPermission,
            Claims.PostAddPermission,
            Claims.PostUpdatePermission,
            Claims.PostDeletePermission,
          }
        },
        {
          CommentManager, 
          new List<string> {
            Claims.CommentAddPermission,
            Claims.CommentUpdatePermission,
            Claims.CommentDeletePermission,
          }
        },
        {
          MessageSender, 
          new List<string> {
            Claims.MessageSendPermission,
          }
        }
      };
  }
}