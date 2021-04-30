using System.Threading;
using System.Threading.Tasks;
using WorkoutApp.Dto;
using WorkoutApp.Entities;

namespace WorkoutApp.Abstractions
{
  public interface IUserRepository
  {
    Task<UserEntity> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UserEntity> UpdateAsync(int id,  UpdateUserDto updateUserDto, CancellationToken cancellationToken);
    Task DeleteAsync(UserEntity user, CancellationToken cancellationToken);
  }
}