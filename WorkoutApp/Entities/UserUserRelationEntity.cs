using System.ComponentModel.DataAnnotations.Schema;
using WorkoutApp.Abstractions;

namespace WorkoutApp.Entities
{
  public class UserUserRelationEntity : IRelationAwareEntity
  {
    public int RequestingUserId { get; set; }
    
    public int RequestedUserId { get; set; } 
      
    [NotMapped]
    public int LeftId
    {
      get => RequestingUserId;
      set => RequestingUserId = value;
    }
    
    [NotMapped]
    public int RightId
    {
      get => RequestedUserId;
      set => RequestedUserId = value;
    }
    
    public UserEntity RequestingUser { get; set; }
    
    public UserEntity RequestedUser { get; set; }
  }
}