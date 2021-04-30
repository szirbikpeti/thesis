using Microsoft.AspNetCore.Identity;

namespace WorkoutApp.Entities
{
  public class UserClaimEntity : IdentityUserClaim<int>
  {
    public UserEntity User { get; set; }
  }
}