using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using WorkoutApp.Abstractions;

namespace WorkoutApp.Entities
{
  public class UserRoleRelationEntity : IdentityUserRole<int>, IRelationAwareEntity
  {
    [NotMapped]
    public int LeftId
    {
      get => UserId;
      set => UserId = value;
    }
    
    [NotMapped]
    public int RightId
    {
      get => RoleId;
      set => RoleId = value;
    }
    
    public UserEntity User { get; set; }
    
    public RoleEntity Role { get; set; }
  }
}