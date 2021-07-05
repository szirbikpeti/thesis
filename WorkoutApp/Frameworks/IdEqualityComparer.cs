using System.Collections.Generic;
using WorkoutApp.Abstractions;

namespace WorkoutApp.Frameworks
{
  public class IdEqualityComparer<TEntity> : IEqualityComparer<TEntity> 
    where TEntity : class, IIdentityAwareEntity 
  {
    public bool Equals(TEntity? x, TEntity? y)
    {
      if (x is null || y is null) {
        return false;
      }

      return x.Id == y.Id;
    }

    public int GetHashCode(TEntity obj) => obj.Id.GetHashCode();
  }
}