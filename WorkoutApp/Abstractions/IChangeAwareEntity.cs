using System;

namespace WorkoutApp.Abstractions
{
  public interface IChangeAwareEntity
  {
    DateTimeOffset CreatedOn { get; set; }
    
    DateTimeOffset ModifiedOn { get; set; }
  }
}