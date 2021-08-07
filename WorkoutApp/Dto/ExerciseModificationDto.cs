using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkoutApp.Dto
{
  public class ExerciseModificationDto
  {
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = null!;

    public string? Equipment { get; set; }

    [Required]
    public ICollection<SetModificationDto> Sets { get; set; } = null!;
  }
}