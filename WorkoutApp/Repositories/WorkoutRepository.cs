using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WorkoutApp.Abstractions;
using WorkoutApp.Data;
using WorkoutApp.Dto;
using WorkoutApp.Entities;
using WorkoutApp.Extensions;

namespace WorkoutApp.Repositories
{
  public class WorkoutRepository : IWorkoutRepository
  {
    private readonly WorkoutDbContext _dbContext;
    private readonly IMapper _mapper;

    public WorkoutRepository(WorkoutDbContext dbContext, IMapper mapper)
    {
      _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ICollection<WorkoutEntity>> ListAsync(int userId, CancellationToken cancellationToken)
    {
      return await _dbContext.Workouts
        .Where(_ => _.UserId == userId)
        .OrderBy(_ => _.Date)
        .ToListAsync(cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task DoAddAsync(WorkoutEntity workout, CancellationToken cancellationToken)
    {
      await _dbContext.Workouts
        .AddAsync(workout, cancellationToken)
        .ConfigureAwait(false);

      await _dbContext
        .SaveChangesAsync(cancellationToken);
    }

    public async Task<WorkoutEntity> DoUpdateAsync(int id,  WorkoutDto workoutDto, CancellationToken cancellationToken)
    {
      var fetchedWorkout = await _dbContext.GetByIdAsync<WorkoutEntity>(id, cancellationToken)
        .ConfigureAwait(false);

      if (fetchedWorkout is null) {
        return null;
      }

      _mapper.Map(workoutDto, fetchedWorkout);

      await _dbContext
        .SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);

      var newlyFetchedUser = await _dbContext.GetByIdAsync<WorkoutEntity>(id, cancellationToken)
        .ConfigureAwait(false);

      return newlyFetchedUser;
    }

    public async Task<bool> DoDeleteAsync(int workoutId, CancellationToken cancellationToken)
    {
      var fetchedWorkout = await _dbContext.GetByIdAsync<WorkoutEntity>(workoutId, cancellationToken)
        .ConfigureAwait(false);

      if (fetchedWorkout is null) {
        return false;
      }
      
      _dbContext.DoDelete(fetchedWorkout);

      return await _dbContext.SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false) > 0;
    }
  }
}