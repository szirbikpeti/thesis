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
    
    Task<ICollection<WorkoutEntity>> ListUnPostedAsync(int userId, CancellationToken cancellationToken);
    
    Task<WorkoutEntity?> DoGetAsync(int workoutId, CancellationToken cancellationToken);
    
    Task DoAddAsync(WorkoutEntity workout, IReadOnlyCollection<int> fileIds, CancellationToken cancellationToken);
    
    Task<WorkoutEntity?> DoUpdateAsync(int workoutId, WorkoutModificationDto workoutDto, CancellationToken cancellationToken);
    
    Task<bool> DoDeleteAsync(int workoutId, CancellationToken cancellationToken);
  }
}