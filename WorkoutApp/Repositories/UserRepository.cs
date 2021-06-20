using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkoutApp.Abstractions;
using WorkoutApp.Data;
using WorkoutApp.Dto;
using WorkoutApp.Entities;
using WorkoutApp.Extensions;

namespace WorkoutApp.Repositories
{
  public class UserRepository : IUserRepository // TODO - delete and extend user manager instead
  {
    private readonly UserManager<UserEntity> _userManager;
    private readonly WorkoutDbContext _dbContext;
    private readonly IMapper _mapper;

    public UserRepository(UserManager<UserEntity> userManager, WorkoutDbContext dbContext, IMapper mapper)
    {
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
      _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<UserEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
      return await _dbContext.GetByIdAsync<UserEntity>(id, cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task<UserEntity?> DoUpdateAsync(UserEntity currentUser, UpdateUserDto updateUserDto,
      CancellationToken cancellationToken)
    {
      _mapper.Map(updateUserDto, currentUser);

      currentUser.ModifiedOn = DateTimeOffset.Now;

      await _dbContext
        .SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);

      var newlyFetchedUser = await _userManager.FindByIdWithAdditionalDataAsync(currentUser.Id, cancellationToken)
        .ConfigureAwait(false);

      return newlyFetchedUser;
    }

    public async Task DoFollowUserAsync(int currentUserId, int requestedUserId, CancellationToken cancellationToken)
    {
      var newEntity = new UserUserRelationEntity {
        LeftId = currentUserId,
        RightId = requestedUserId
      };

      await _dbContext.UserUserRelations
        .AddAsync(newEntity, cancellationToken)
        .ConfigureAwait(false);
      
      await _dbContext
        .SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task DoUnFollowUserAsync(int currentUserId, int requestedUserId, CancellationToken cancellationToken)
    {
      var removedEntity = await _dbContext.UserUserRelations
        .Where(_ => _.RequestingUserId == currentUserId 
                    && _.RequestedUserId == requestedUserId)
        .FirstOrDefaultAsync(cancellationToken);

      _dbContext.UserUserRelations.Remove(removedEntity!);
      
      await _dbContext
        .SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);
    }

  public async Task<bool> DoDeleteAsync(int userId, CancellationToken cancellationToken)
    {
      var fetchedUser = await _dbContext.GetByIdAsync<UserEntity>(userId, cancellationToken)
        .ConfigureAwait(false);

      if (fetchedUser is null) {
        return false;
      }
      
      _dbContext.DoDelete(fetchedUser);

      return await _dbContext.SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false) > 0;
    }
  }
}