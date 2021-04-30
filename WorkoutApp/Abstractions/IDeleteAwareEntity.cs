using System;

namespace WorkoutApp.Abstractions
{
  public interface IDeleteAwareEntity
  {
    DateTimeOffset? DeletedOn { get; set; }
  }
}