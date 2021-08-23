using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkoutApp.Abstractions;
using WorkoutApp.Data;
using WorkoutApp.Entities;

namespace WorkoutApp.Repositories
{
  public class FeedbackRepository : IFeedbackRepository
  {
    private readonly WorkoutDbContext _dbContext;
    
    public FeedbackRepository(
      WorkoutDbContext dbContext)
    {
      _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<ICollection<FeedbackEntity>> DoListAsync(CancellationToken cancellationToken)
    {
      return await _dbContext.Feedbacks
        .Include(_ => _.User)
        .ThenInclude(_ => _.ProfilePicture)
        .OrderByDescending(_ => _.CreatedOn)
        .ToListAsync(cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task DoAddAsync(FeedbackEntity feedback, CancellationToken cancellationToken)
    {
      await _dbContext.Feedbacks
        .AddAsync(feedback, cancellationToken)
        .ConfigureAwait(false);

      await _dbContext
        .SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);
    }
  }
}