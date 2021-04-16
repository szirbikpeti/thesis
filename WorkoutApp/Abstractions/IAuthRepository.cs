using System.Threading;
using System.Threading.Tasks;
using WorkoutApp.Entities;

namespace WorkoutApp.Abstractions
{
  public interface IAuthRepository
  {
    
    Task<UserEntity> SignUpAsync(UserEntity user, string password, CancellationToken cancellationToken);
    
    Task<UserEntity> SignInAsync(string userName, string password, CancellationToken cancellationToken);
    
    Task<bool> IsUserExistsAsync(string userName, CancellationToken cancellationToken);
  }
}