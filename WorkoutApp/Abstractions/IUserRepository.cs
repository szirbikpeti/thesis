using System.Threading;
using System.Threading.Tasks;
using WorkoutApp.Dto;
using WorkoutApp.Entities;

namespace WorkoutApp.Abstractions
{
  public interface IUserRepository
  {
    Task<UserEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UserEntity?> UpdateAsync(UserEntity currentUser,  UpdateUserDto updateUserDto, CancellationToken cancellationToken);
    Task<bool> DoDeleteAsync(int userId, CancellationToken cancellationToken);
  }
}