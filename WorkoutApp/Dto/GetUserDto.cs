using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace WorkoutApp.Dto
{
    public class GetUserDto : BaseUserDto
    {
        public int Id { get; set; }

        public string About { get; set; } = null!;

        public GetFileDto ProfilePicture { get; set; } = null!;
        
        public DateTimeOffset LastSignedInOn { get; set; }

        public IReadOnlyCollection<string> Roles { get; set; } = ImmutableList<string>.Empty;
        
        public IReadOnlyCollection<string> Permissions { get; set; } = ImmutableList<string>.Empty;
        
        public IReadOnlyCollection<int> SourceUserIds { get; set; } = ImmutableList<int>.Empty;
        
        public IReadOnlyCollection<int> TargetUserIds { get; set; } = ImmutableList<int>.Empty;
        
        public IReadOnlyCollection<int> FollowerUserIds { get; set; } = ImmutableList<int>.Empty;
        
        public IReadOnlyCollection<int> FollowedUserIds { get; set; } = ImmutableList<int>.Empty;
    }
}