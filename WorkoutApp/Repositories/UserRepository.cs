using System;
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
  public class UserRepository : IUserRepository
  {
    
    private readonly WorkoutDbContext _dbContext;
    
    public UserRepository(WorkoutDbContext dbContext)
    {
      _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<UserEntity> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
      return await _dbContext.Users
        .FirstOrDefaultAsync(_ => _.Id == id, cancellationToken)
        .ConfigureAwait(false);
    }
    
    
  }
}