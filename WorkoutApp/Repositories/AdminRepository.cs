using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkoutApp.Abstractions;
using WorkoutApp.Data;
using WorkoutApp.Dto;
using WorkoutApp.Entities;

namespace WorkoutApp.Repositories
{
  public class AdminRepository: IAdminRepository
  {
    private readonly WorkoutDbContext _dbContext;
    
    public AdminRepository(WorkoutDbContext dbContext)
    {
      _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }
    
    public async Task<ICollection<UserEntity>> ListAsync(CancellationToken cancellationToken)
    {
      return await _dbContext.Users
        .OrderBy(_ => _.UserName)
        .ToListAsync(cancellationToken)
        .ConfigureAwait(false);
    }

    public Task<GetUserDto> BlockUserAsync(CancellationToken cancellationToken) => throw new System.NotImplementedException();
  }
}