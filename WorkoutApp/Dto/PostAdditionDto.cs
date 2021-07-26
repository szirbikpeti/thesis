using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace WorkoutApp.Dto
{
  public class PostAdditionDto
  {
    [Required]
    public int WorkoutId { get; set; }
    
    [Required]
    public string Description { get; set; } = null!;
    
    [Required]
    public IReadOnlyCollection<int> FileIds { get; set; } = ImmutableList<int>.Empty;
  }
}