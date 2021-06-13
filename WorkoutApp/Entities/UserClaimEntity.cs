using Microsoft.AspNetCore.Identity;

namespace WorkoutApp.Entities
{
  public sealed class UserClaimEntity : IdentityUserClaim<int>
  {
    public UserEntity User { get; set; } = null!;
  }
}