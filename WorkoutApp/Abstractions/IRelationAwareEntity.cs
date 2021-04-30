namespace WorkoutApp.Abstractions
{
  public interface IRelationAwareEntity
  {
    int LeftId { get; set; }
    
    int RightId { get; set; }
  }
}