using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkoutApp.Abstractions;
using WorkoutApp.Dto;
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
        .OrderBy(_ => _.UserName)
        .ToListAsync(cancellationToken)
        .ConfigureAwait(false);
    }

    public Task<GetUserDto> BlockUserAsync(CancellationToken cancellationToken)
    {
      // TODO - implement it

      throw new NotImplementedException();
    }
  }
}