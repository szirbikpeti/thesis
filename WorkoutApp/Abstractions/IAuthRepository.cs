using System.Threading;
using System.Threading.Tasks;
using WorkoutApp.Entities;

namespace WorkoutApp.Abstractions
{
  public interface IAuthRepository
  {
    Task<bool> IsUserExistsAsync(string userName, CancellationToken cancellationToken);
    
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
  }
}