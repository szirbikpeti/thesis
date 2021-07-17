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

    [Required]
    public ICollection<ExerciseModificationDto> Exercises { get; set; } = null!;
    
    public IReadOnlyCollection<int> FileIds { get; set; } = ImmutableList<int>.Empty;
  }
}