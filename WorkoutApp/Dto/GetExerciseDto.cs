using System.Collections.Generic;

namespace WorkoutApp.Dto
{
  public class GetExerciseDto
  {
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;

    public string? Equipment { get; set; }

    public ICollection<GetSetDto> Sets { get; set; } = null!;
  }
}