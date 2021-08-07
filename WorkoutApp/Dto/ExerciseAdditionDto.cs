using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkoutApp.Dto
{
  public class ExerciseAdditionDto
  {
    [Required]
    public string Name { get; set; } = null!;

    public string? Equipment { get; set; }

    [Required]
    public ICollection<SetAdditionDto> Sets { get; set; } = null!;
  }
}