using System;
using System.Collections;
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

    private async Task<WorkoutEntity?> FindWorkoutByIdWithAdditionalDataAsync(int id, CancellationToken cancellationToken)
    {
      return await _dbContext.Workouts
        .AsNoTracking()
        .AsSplitQuery()
        .Include(_ => _.Exercises)
        .ThenInclude(_ => _.Sets)
        .Include(_ => _.FileRelationEntities)
        .FirstOrDefaultAsync(_ => _.Id == id, cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task<ICollection<WorkoutEntity>> ListAsync(int userId, CancellationToken cancellationToken)
    {
      return await _dbContext.Workouts
        .AsNoTracking()
        .AsSplitQuery()
        .Where(_ => _.UserId == userId)
        .Include(_ => _.Exercises)
        .ThenInclude(_ => _.Sets)
        .Include(_ => _.FileRelationEntities)
        .ThenInclude(_ => _.File)
        .OrderByDescending(_ => _.Date)
        .ToListAsync(cancellationToken)
        .ConfigureAwait(false);
    }
    
    public async Task<WorkoutEntity?> DoGetAsync(int workoutId, CancellationToken cancellationToken)
    {
      return await _dbContext.Workouts
        .AsNoTracking()
        .AsSplitQuery()
        .Where(_ => _.Id == workoutId)
        .Include(_ => _.Exercises)
        .ThenInclude(_ => _.Sets)
        .Include(_ => _.FileRelationEntities)
        .ThenInclude(_ => _.File)
        .FirstOrDefaultAsync(cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task DoAddAsync(WorkoutEntity workout, IReadOnlyCollection<int> fileIds, CancellationToken cancellationToken)
    {
      await _dbContext.Workouts
        .AddAsync(workout, cancellationToken)
        .ConfigureAwait(false);
      
      await _dbContext
        .SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);

      if (fileIds.Count > 0) {
        var createdWorkout = await _dbContext.Workouts.Where(_ => _.CreatedOn == workout.CreatedOn)
          .FirstOrDefaultAsync(cancellationToken)
          .ConfigureAwait(false);
        
        var newEntities = fileIds.Select(_ => new WorkoutFileRelationEntity {
          LeftId = createdWorkout!.Id,
          RightId = _
        });

        await _dbContext.WorkoutFileRelations
          .AddRangeAsync(newEntities, cancellationToken)
          .ConfigureAwait(false);

        await _dbContext
          .SaveChangesAsync(cancellationToken)
          .ConfigureAwait(false);
      }
    }

    public async Task<WorkoutEntity?> DoUpdateAsync(int id, WorkoutModificationDto workoutDto, CancellationToken cancellationToken)
    {
      var fetchedWorkout = await FindWorkoutByIdWithAdditionalDataAsync(id, cancellationToken)
        .ConfigureAwait(false);

      if (fetchedWorkout is null) {
        return null;
      }

      _mapper.Map(workoutDto, fetchedWorkout);
      
      fetchedWorkout.ModifiedOn = DateTimeOffset.Now;

      var removedFileIds = fetchedWorkout.FileRelationEntities.Select(_ => _.FileId).Except(workoutDto.FileIds);
      var removedEntities = fetchedWorkout.FileRelationEntities
        .Where(_ => _.WorkoutId == fetchedWorkout.Id && removedFileIds.Contains(_.FileId)); 
      
      _dbContext.WorkoutFileRelations.RemoveRange(removedEntities);

      var newFileIds = workoutDto.FileIds.Except(fetchedWorkout.FileRelationEntities.Select(_ => _.FileId));
      var newEntities = newFileIds.Select(_ => new WorkoutFileRelationEntity {
        LeftId = fetchedWorkout.Id,
        RightId = _
      });
      
      await _dbContext.WorkoutFileRelations
        .AddRangeAsync(newEntities, cancellationToken)
        .ConfigureAwait(false);
      
      await _dbContext
        .SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);

      var newlyFetchedWorkout = await _dbContext.GetByIdAsync<WorkoutEntity>(id, cancellationToken)
        .ConfigureAwait(false);

      return newlyFetchedWorkout;
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