using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
      CancellationToken cancellationToken)
    {
      return await userManager.Users
        .AsNoTracking()
        .AsSplitQuery()
        .Include(_ => _.Roles)
        .ThenInclude(_ => _.Role)
        .ThenInclude(_ => _.Claims)
        .Include(_ => _.RequestingUsers)
        .Include(_ => _.RequestedUsers)
        .FirstOrDefaultAsync(_ => 
          (_.Id == id) && (_.DeletedOn == null), cancellationToken)
        .ConfigureAwait(false);
    }
    
    public static async Task<UserEntity?> FindByNameWithAdditionalDataAsync(
      this UserManager<UserEntity> userManager, 
      string userName,
      CancellationToken cancellationToken)
    {
      var normalizedUserName = userManager.NormalizeName(userName);
      
      return await userManager.Users
        .AsNoTracking()
        .AsSplitQuery()
        .Include(_ => _.Roles)
        .ThenInclude(_ => _.Role)
        .ThenInclude(_ => _.Claims)
        .Include(_ => _.RequestingUsers)
        .Include(_ => _.RequestedUsers)
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
          && _.DeletedOn == null)
        .Include(_ => _.RequestingUsers)
        .Include(_ => _.RequestedUsers)
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