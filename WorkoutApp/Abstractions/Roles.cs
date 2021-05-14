using System.Collections.Generic;

namespace WorkoutApp.Abstractions
{
  public class Roles
  {
    public const string Administrator = "Administrator";
    public const string User = "User";

    public static Dictionary<string, List<string>> GetRolePermissions() =>
      new() {
        {
          Administrator, 
          new List<string> {
            Claims.UserManagementPermission,
            Claims.PostListPermission,
            Claims.CommentAddPermission,
          }
        },
        {
          User, 
          new List<string> {
            Claims.WorkoutManagementPermission,
            Claims.PostManagementPermission,
            Claims.CommentAddPermission,
            Claims.MessageSendPermission,
          }
        }
      };
  }
}