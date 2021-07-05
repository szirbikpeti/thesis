using System.Collections.Generic;
using System.Collections.Immutable;

namespace WorkoutApp.Dto
{
  public class GetFriendsDto
  {
    public ICollection<GetUserDto> FollowerUsers { get; set; } = ImmutableList<GetUserDto>.Empty;
    public ICollection<GetUserDto> FollowedUsers { get; set; } = ImmutableList<GetUserDto>.Empty;
  }
}