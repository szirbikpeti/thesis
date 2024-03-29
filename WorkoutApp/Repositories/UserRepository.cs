﻿using System;
using System.Collections.Generic;
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
  public class UserRepository : IUserRepository
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

      var newlyFetchedUser = await _userManager.FindByIdWithAdditionalDataAsync(
          currentUser.Id, includesFollowsData: false, includesProfilePicture: true, cancellationToken: cancellationToken)
        .ConfigureAwait(false);

      return newlyFetchedUser;
    }

    public async Task<ICollection<UserEntity>> DoListFollowedUsersAsync(
      int currentUserId,
      CancellationToken cancellationToken)
    {
      return await _dbContext.Follows
        .AsNoTracking()
        .Where(_ => _.FollowerId == currentUserId)
        .Include(_ => _.FollowedUser)
        .ThenInclude(_ => _.ProfilePicture)
        .Select(_ => _.FollowedUser)
        .ToListAsync(cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task<ICollection<UserEntity>> DoListFollowerUsersAsync(
      int currentUserId,
      CancellationToken cancellationToken)
    {
      return await _dbContext.Follows
        .AsNoTracking()
        .Where(_ => _.FollowedId == currentUserId)
        .Include(_ => _.FollowerUser)
        .ThenInclude(_ => _.ProfilePicture)
        .Select(_ => _.FollowerUser)
        .ToListAsync(cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task DoAddFollowRequestAsync(int currentUserId, int requestedUserId, CancellationToken cancellationToken)
    {
      var newEntity = new FollowRequestEntity {
        SourceId = currentUserId,
        TargetId = requestedUserId
      };

      await _dbContext.FollowRequests
        .AddAsync(newEntity, cancellationToken)
        .ConfigureAwait(false);
      
      await _dbContext
        .SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task DoAcceptFollowRequestAsync(int currentUserId, int followerId, CancellationToken cancellationToken)
    {
      var entity = await _dbContext.FollowRequests
        .Where(_ => _.SourceId == followerId 
                    && _.TargetId == currentUserId)
        .FirstOrDefaultAsync(cancellationToken)
        .ConfigureAwait(false);

      _dbContext.FollowRequests.Remove(entity!);
      
      var newEntity = new FollowEntity {
        FollowerId = followerId,
        FollowedId = currentUserId
      };

      await _dbContext.Follows
        .AddAsync(newEntity, cancellationToken)
        .ConfigureAwait(false);
      
      await _dbContext
        .SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task DoFollowBackAsync(int currentUserId, int followedId, CancellationToken cancellationToken)
    {
      var newEntity = new FollowEntity {
        FollowerId = currentUserId,
        FollowedId = followedId
      };

      await _dbContext.Follows
        .AddAsync(newEntity, cancellationToken)
        .ConfigureAwait(false);
      
      await _dbContext
        .SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task DoDeclineFollowRequest(int currentUserId, int followerId, CancellationToken cancellationToken)
    {
      var entity = await _dbContext.FollowRequests
        .Where(_ => _.SourceId == followerId 
                    && _.TargetId == currentUserId)
        .FirstOrDefaultAsync(cancellationToken)
        .ConfigureAwait(false);

      entity!.IsBlocked = true;

      _dbContext.FollowRequests.Update(entity!);

      await _dbContext
        .SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task DoDeleteFollowRequestAsync(int sourceId, int targetId, CancellationToken cancellationToken)
    {
      var removedEntity = await _dbContext.FollowRequests
        .Where(_ => _.SourceId == sourceId 
                    && _.TargetId == targetId)
        .FirstOrDefaultAsync(cancellationToken)
        .ConfigureAwait(false);

      _dbContext.FollowRequests.Remove(removedEntity!);
      
      await _dbContext
        .SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);
    }
    
    public async Task DoUnFollowAsync(int currentUserId, int followedId, CancellationToken cancellationToken)
    {
      var removedEntity = await _dbContext.Follows
        .Where(_ => _.FollowerId == currentUserId 
                    && _.FollowedId == followedId)
        .FirstOrDefaultAsync(cancellationToken)
        .ConfigureAwait(false);

      _dbContext.Follows.Remove(removedEntity!);
      
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