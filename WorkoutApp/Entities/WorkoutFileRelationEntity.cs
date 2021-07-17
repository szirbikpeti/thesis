using System.ComponentModel.DataAnnotations.Schema;
using WorkoutApp.Abstractions;

namespace WorkoutApp.Entities
{
  public sealed class WorkoutFileRelationEntity
  {
    public int WorkoutId { get; set; }

    public int FileId { get; set; }

    public WorkoutEntity Workout { get; set; } = null!;

    public FileEntity File { get; set; } = null!;
  }
}