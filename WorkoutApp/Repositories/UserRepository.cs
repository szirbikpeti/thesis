using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WorkoutApp.Abstractions;
using WorkoutApp.Data;
using WorkoutApp.Dto;
using WorkoutApp.Entities;
using WorkoutApp.Extensions;

namespace WorkoutApp.Repositories
{
  public class UserRepository : IUserRepository
  {
    private readonly WorkoutDbContext _dbContext;
    private readonly IMapper _mapper;
    
    public UserRepository(WorkoutDbContext dbContext, IMapper mapper)
    {
      _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<UserEntity> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
      return await _dbContext.GetByIdAsync<UserEntity>(id, cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task<UserEntity> UpdateAsync(int id,  UpdateUserDto updateUserDto, CancellationToken cancellationToken)
    {
      var fetchedUser = await _dbContext.GetByIdAsync<UserEntity>(id, cancellationToken).ConfigureAwait(false);

      if (fetchedUser is null) {
        return null;
      }

      _mapper.Map(updateUserDto, fetchedUser);

      await _dbContext
        .SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);

      var newlyFetchedUser = await _dbContext.GetByIdAsync<UserEntity>(id, cancellationToken).ConfigureAwait(false);

      return newlyFetchedUser;
    }

    public async Task<bool> DoDeleteAsync(int userId, CancellationToken cancellationToken)
    {
      var fetchedUser = await _dbContext.GetByIdAsync<UserEntity>(userId, cancellationToken)
        .ConfigureAwait(false);

      if (fetchedUser is null) {
        return false;
      }
      
      _dbContext.Remove(fetchedUser);

      return await _dbContext.SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false) > 0;
    }
  }
}