using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using WorkoutApp.Abstractions;

namespace WorkoutApp.Entities
{
  public class RoleEntity : IdentityRole<int>, IIdentityAwareEntity
  {
    public ICollection<RoleClaimEntity> Claims { get; set; } = null!;
    
    public ICollection<UserRoleRelationEntity> Users { get; set; } = null!;
  }
}