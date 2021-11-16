using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkoutApp.Abstractions;
using WorkoutApp.Entities;

namespace WorkoutApp.Extensions
{
  public static class UserManagerExtension
  {
    public static int GetUserIdAsInt(
      this UserManager<UserEntity> userManager, 
      ClaimsPrincipal principal) => int.Parse(userManager.GetUserId(principal));

    public static async Task<UserEntity?> FindByIdWithAdditionalDataAsync(
      this UserManager<UserEntity> userManager, 
      int id,
      bool includesRoles = true,
      bool includesFollowsData = true,
      bool includesProfilePicture = false,
      CancellationToken cancellationToken = default)
    {
      var fetchedUser = userManager.Users
        .AsSplitQuery()
        .Where(_ => _.Id == id && _.DeletedOn == null);

      if (includesRoles) {
        fetchedUser = fetchedUser
          .Include(_ => _.Roles)
          .ThenInclude(_ => _.Role)
          .ThenInclude(_ => _.Claims);
      }

      if (includesFollowsData) {
        fetchedUser = fetchedUser
          .Include(_ => _.SourceUsers)
          .Include(_ => _.TargetUsers)
          .Include(_ => _.FollowerUsers)
          .Include(_ => _.FollowedUsers);
      }

      if (includesProfilePicture) {
        fetchedUser = fetchedUser
          .Include(_ => _.ProfilePicture);
      }

      return await fetchedUser
        .FirstOrDefaultAsync(cancellationToken)
        .ConfigureAwait(false);
    }
    
    public static async Task<UserEntity?> FindByNameWithAdditionalDataAsync(
      this UserManager<UserEntity> userManager, 
      string userName,
      CancellationToken cancellationToken)
    {
      var normalizedUserName = userManager.NormalizeName(userName);
      
      return await userManager.Users
        .AsSplitQuery()
        .Include(_ => _.ProfilePicture)
        .Include(_ => _.Roles)
        .ThenInclude(_ => _.Role)
        .ThenInclude(_ => _.Claims)
        .FirstOrDefaultAsync(_ => 
          (_.NormalizedUserName == normalizedUserName) 
            && (_.DeletedOn == null), 
          cancellationToken)
        .ConfigureAwait(false);
    }
    
    public static async Task<ICollection<UserEntity>> ListByUserAndFullNameAsync(
      this UserManager<UserEntity> userManager,
      int currentUserId,
      string name, 
      CancellationToken cancellationToken)
    {
      var normalizedName = userManager.NormalizeName(name);
      
      return await userManager.Users
        .AsNoTracking()
        .Where(_ =>
          (_.FullName.ToUpper().Contains(normalizedName) 
           || _.NormalizedUserName.Contains(normalizedName)) 
          && _.Id != currentUserId
          && _.LockoutEnabled)
        .Include(_ => _.ProfilePicture)
        .ToListAsync(cancellationToken)
        .ConfigureAwait(false);
    }

    public static async Task<ICollection<int>> ListFollowedUserIdsAsync(
      this UserManager<UserEntity> userManager,
      int currentUserId,
      CancellationToken cancellationToken)
    {
      return await userManager.Users
        .AsNoTracking()
        .Where(_ =>_.Id == currentUserId 
          && _.DeletedOn == null)
        .Include(_ => _.FollowedUsers)
        .SelectMany(_ => _.FollowedUsers)
        .Select(_ => _.FollowedUser.Id)
        .ToListAsync(cancellationToken)
        .ConfigureAwait(false);
    }

    public static async Task<bool> IsUserExistsAsync(
      this UserManager<UserEntity> userManager, 
      string userName, 
      CancellationToken cancellationToken)
    {
      var normalizedUserName = userManager.NormalizeName(userName);
      
      return await userManager.Users
        .AnyAsync(_ => 
          (_.NormalizedUserName == normalizedUserName) 
            && (_.DeletedOn == null), 
          cancellationToken)
        .ConfigureAwait(false);
    }
  }
}