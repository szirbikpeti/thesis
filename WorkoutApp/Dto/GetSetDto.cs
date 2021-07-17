using System;

namespace WorkoutApp.Dto
{
  public class GetSetDto
  {
    public int Id { get; set; }
    
    public int Reps { get; set; }

    public double Weight { get; set; }
        
    public TimeSpan? Duration { get; set; }
  }
}