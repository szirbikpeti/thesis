using System;
using System.ComponentModel.DataAnnotations;

namespace WorkoutApp.Dto
{
  public class SetDto
  {
    [Required]
    public int Reps { get; set; }

    [Required]
    public double Weight { get; set; }
        
    public TimeSpan? Duration { get; set; }
  }
}