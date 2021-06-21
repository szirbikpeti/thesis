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

    Task DoAddFollowRequestAsync(int currentUserId, int requestedUserId, CancellationToken cancellationToken);
    
    Task DoAcceptFollowRequestAsync(int currentUserId, int followerId, CancellationToken cancellationToken);
    
    Task DoFollowBackAsync(int currentUserId, int followedId, CancellationToken cancellationToken);
    
    Task DoDeclineFollowRequest(int currentUserId, int followerId, CancellationToken cancellationToken);
    
    Task DoDeleteFollowRequestAsync(int currentUserId, int targetId, CancellationToken cancellationToken);
    
    Task DoUnFollowAsync(int currentUserId, int followedId, CancellationToken cancellationToken);
    
    Task<bool> DoDeleteAsync(int userId, CancellationToken cancellationToken);
  }
}