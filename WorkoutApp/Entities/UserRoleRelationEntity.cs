using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using WorkoutApp.Abstractions;

namespace WorkoutApp.Entities
{
  public sealed class UserRoleRelationEntity : IdentityUserRole<int>, IRelationAwareEntity
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

    public UserEntity User { get; set; } = null!;
    
    public RoleEntity Role { get; set; } = null!;
  }
}