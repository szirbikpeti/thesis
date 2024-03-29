﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace WorkoutApp.Dto
{
    public class GetUserDto : BaseUserDto
    {
        public int Id { get; set; }

        public string About { get; set; } = null!;

        public bool IsBlocked { get; set; }

        public GetFileDto ProfilePicture { get; set; } = null!;
        
        public DateTimeOffset LastSignedInOn { get; set; }

        public IReadOnlyCollection<string> Roles { get; set; } = ImmutableList<string>.Empty;
        
        public IReadOnlyCollection<string> Permissions { get; set; } = ImmutableList<string>.Empty;
    }
}