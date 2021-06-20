using System.Threading;
using System.Threading.Tasks;
using WorkoutApp.Dto;
using WorkoutApp.Entities;

namespace WorkoutApp.Abstractions
{
  public interface IUserRepository
  {
    Task<UserEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    
    Task<UserEntity?> DoUpdateAsync(UserEntity currentUser,  UpdateUserDto updateUserDto, CancellationToken cancellationToken);

    Task DoFollowUserAsync(int currentUserId, int requestedUserId, CancellationToken cancellationToken);
    
    Task DoUnFollowUserAsync(int currentUserId, int requestedUserId, CancellationToken cancellationToken);
    
    Task<bool> DoDeleteAsync(int userId, CancellationToken cancellationToken);
  }
}