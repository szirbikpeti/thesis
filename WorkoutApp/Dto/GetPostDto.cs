using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace WorkoutApp.Dto
{
  public class GetPostDto
  {
    public int Id { get; set; }
    
    public DateTimeOffset PostedOn { get; set; }

    public string Description { get; set; } = null!;

    public GetWorkoutDto Workout { get; set; } = null!;
    
    public GetUserDto User { get; set; } = null!;
    
    public IReadOnlyCollection<GetFileDto> Files { get; set; } = 
      ImmutableList<GetFileDto>.Empty;
    
    public IReadOnlyCollection<GetCommentDto> Comments { get; set; } = 
      ImmutableList<GetCommentDto>.Empty;
    
    public IReadOnlyCollection<GetUserDto> LikingUsers { get; set; } = 
      ImmutableList<GetUserDto>.Empty;
  }
}