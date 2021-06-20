using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace WorkoutApp.Dto
{
  public class WorkoutAdditionDto
  {
    [Required]
    public DateTimeOffset Date { get; set; }

    [Required]
    public string Type { get; set; } = null!;

    [Required]
    public ICollection<ExerciseDto> Exercises { get; set; } = null!;
    
    public IReadOnlyCollection<int> FileIds { get; set; } = ImmutableList<int>.Empty;
  }
}