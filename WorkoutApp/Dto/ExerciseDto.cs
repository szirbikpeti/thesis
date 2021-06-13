using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkoutApp.Dto
{
  public class ExerciseDto
  {
    [Required]
    public string Name { get; set; } = null!;

    public string Equipment { get; set; } = null!;

    [Required] public ICollection<SetDto> Sets { get; set; } = null!;
  }
}