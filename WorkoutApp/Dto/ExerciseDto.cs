using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkoutApp.Dto
{
  public class ExerciseDto
  {
    [Required]
    public string Name { get; set; }

    public string Equipment { get; set; }

    [Required]
    public ICollection<SetDto> Sets { get; set; }
  }
}