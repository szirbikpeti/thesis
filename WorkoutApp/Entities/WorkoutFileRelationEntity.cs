using System.ComponentModel.DataAnnotations.Schema;
using WorkoutApp.Abstractions;

namespace WorkoutApp.Entities
{
  public sealed class WorkoutFileRelationEntity : IRelationAwareEntity
  {
    public int WorkoutId { get; set; }

    public int FileId { get; set; }

    [NotMapped]
    public int LeftId
    {
      get => WorkoutId;
      set => WorkoutId = value;
    }

    [NotMapped]
    public int RightId
    {
      get => FileId;
      set => FileId = value;
    }

    public WorkoutEntity Workout { get; set; } = null!;

    public FileEntity File { get; set; } = null!;
  }
}