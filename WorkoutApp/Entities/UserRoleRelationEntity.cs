using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using WorkoutApp.Abstractions;

namespace WorkoutApp.Entities
{
  public sealed class UserRoleRelationEntity : IdentityUserRole<int>
  {
    public UserEntity User { get; set; } = null!;
    
    public RoleEntity Role { get; set; } = null!;
  }
}