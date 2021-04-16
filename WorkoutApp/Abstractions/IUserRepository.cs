using System.Threading;
using System.Threading.Tasks;
using WorkoutApp.Entities;

namespace WorkoutApp.Abstractions
{
  public interface IUserRepository
  {
    Task<UserEntity> GetByIdAsync(int id, CancellationToken cancellationToken = default);
  }
}