﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkoutApp.Abstractions;
using WorkoutApp.Data;

namespace WorkoutApp.Extensions
{
  public static class WorkoutDbContextExtension
  {
    public static async Task<TEntity?> GetByIdAsync<TEntity>(
      this WorkoutDbContext dbContext, int id, 
      CancellationToken cancellationToken) 
      where TEntity : class, IIdentityAwareEntity
    {
      return await dbContext.Set<TEntity>()
        .FirstOrDefaultAsync(_ => _.Id == id, cancellationToken)
        .ConfigureAwait(false);
    }
    
    public static void DoDelete<TEntity>(this WorkoutDbContext dbContext, TEntity entity)
      where TEntity : class, IDeleteAwareEntity
    {
      if (entity is null) {
        throw new ArgumentNullException(nameof(entity));
      }

      entity.DeletedOn = DateTimeOffset.Now;

      dbContext.Set<TEntity>().Update(entity);
    }
    
    public static void DoDeleteRange<TEntity>(this WorkoutDbContext dbContext, ICollection<TEntity> entities)
      where TEntity : class, IDeleteAwareEntity
    {
      foreach (var entity in entities) {
        entity.DeletedOn = DateTimeOffset.Now;
      }

      dbContext.Set<TEntity>().UpdateRange(entities);
    }
  }
}