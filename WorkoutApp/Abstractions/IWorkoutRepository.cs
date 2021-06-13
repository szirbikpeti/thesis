using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WorkoutApp.Dto;
using WorkoutApp.Entities;

namespace WorkoutApp.Abstractions
{
  public interface IWorkoutRepository
  {

    Task<ICollection<WorkoutEntity>> ListAsync(int userId, CancellationToken cancellationToken);
    
    Task DoAddAsync(WorkoutEntity workout, CancellationToken cancellationToken);
    
    Task<WorkoutEntity?> DoUpdateAsync(int workoutId, WorkoutDto workoutDto, CancellationToken cancellationToken);
    
    Task<bool> DoDeleteAsync(int workoutId, CancellationToken cancellationToken);
  }
}