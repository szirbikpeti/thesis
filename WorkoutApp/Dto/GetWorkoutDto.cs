using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace WorkoutApp.Dto
{
  public class GetWorkoutDto
  {
    public int Id { get; set; }
    
    public DateTimeOffset Date { get; set; }

    public string Type { get; set; } = null!;
    
    public DateTimeOffset CreatedOn { get; set; }
        
    public DateTimeOffset ModifiedOn { get; set; }
    
    public GetPostDto? RelatedPost { get; set; }

    public IReadOnlyCollection<GetExerciseDto> Exercises { get; set; } = 
      ImmutableList<GetExerciseDto>.Empty;
    
    public IReadOnlyCollection<GetFileDto> Files { get; set; } = 
      ImmutableList<GetFileDto>.Empty;
  }
}