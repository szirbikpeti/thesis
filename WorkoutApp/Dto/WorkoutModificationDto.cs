using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace WorkoutApp.Dto
{
  public class WorkoutModificationDto
  {
    public int Id {get; set; }
    
    [Required]
    public DateTimeOffset Date { get; set; }

    [Required]
    public string Type { get; set; } = null!;

    public double? Distance { get; set; }
        
    public string? Duration { get; set; }

    public ICollection<ExerciseAdditionDto> Exercises { get; set; }
      = ImmutableList<ExerciseAdditionDto>.Empty;
    
    public IReadOnlyCollection<int> FileIds { get; set; } = ImmutableList<int>.Empty;
  }
}