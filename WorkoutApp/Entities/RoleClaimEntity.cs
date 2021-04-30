using Microsoft.AspNetCore.Identity;

namespace WorkoutApp.Entities
{
  public class RoleClaimEntity : IdentityRoleClaim<int>
  {
    public RoleEntity Role { get; set; }
  }
}