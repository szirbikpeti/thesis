using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkoutApp.Abstractions;
using WorkoutApp.Entities;

namespace WorkoutApp.Repositories
{
  public class AdminRepository: IAdminRepository
  {
    private readonly UserManager<UserEntity> _userManager;
    
    public AdminRepository(UserManager<UserEntity> userManager)
    {
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }
    
    public async Task<ICollection<UserEntity>> ListAsync(CancellationToken cancellationToken)
    {
      return await _userManager.Users
        .AsSplitQuery()
        .Include(_ => _.ProfilePicture)
        .Include(_ => _.Roles)
        .ThenInclude(_ => _.Role)
        .ThenInclude(_ => _.Claims)
        .OrderBy(_ => _.UserName)
        .ToListAsync(cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task<bool> DoBlockUserAsync(UserEntity user, CancellationToken cancellationToken)
    {
      user.LockoutEnd = new DateTime(2099, 12, 31, 23, 59, 59, DateTimeKind.Utc);

      var result = await _userManager.UpdateAsync(user)
        .ConfigureAwait(false);

      return result.Succeeded;
    }

    public async Task<bool> DoRestoreUserAsync(UserEntity user, CancellationToken cancellationToken)
    {
      user.LockoutEnd = null;

      var result = await _userManager.UpdateAsync(user)
        .ConfigureAwait(false);

      return result.Succeeded;
    }
  }
}