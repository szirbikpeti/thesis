using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkoutApp.Abstractions;
using WorkoutApp.Data;
using WorkoutApp.Entities;

namespace WorkoutApp.Repositories
{
  public class AuthRepository : IAuthRepository
  {
    private readonly WorkoutDbContext _dbContext;

    public AuthRepository(WorkoutDbContext dbContext)
    {
      _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<bool> IsUserExistsAsync(string userName, CancellationToken cancellationToken)
    {
      return await _dbContext.Users
        .AnyAsync(_ => _.UserName.Equals(userName), cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken) 
      => await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0;
  }
}