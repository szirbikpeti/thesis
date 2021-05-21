using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkoutApp.Dto
{
  public class WorkoutDto
  {
    [Required]
    public DateTimeOffset Date { get; set; }

    [Required]
    public string Type { get; set; }

    [Required]
    public ICollection<ExerciseDto> Exercises { get; set; }
  }
}