using System.Collections.Generic;
using System.Collections.Immutable;

namespace WorkoutApp.Dto
{
  public class GetFollowRequestsAndFollowsDto
  {
    public ICollection<GetFollowRequestDto> SourceUsers { get; set; }
      = ImmutableList<GetFollowRequestDto>.Empty;
    
    public IReadOnlyCollection<GetFollowRequestDto> TargetUsers { get; set; }
      = ImmutableList<GetFollowRequestDto>.Empty;

    public IReadOnlyCollection<int> FollowerUserIds { get; set; }
      = ImmutableList<int>.Empty;
        
    public IReadOnlyCollection<int> FollowedUserIds { get; set; } 
      = ImmutableList<int>.Empty;
  }
}