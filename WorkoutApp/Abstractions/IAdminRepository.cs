﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WorkoutApp.Dto;
using WorkoutApp.Entities;

namespace WorkoutApp.Abstractions
{
  public interface IAdminRepository
  {
    Task<ICollection<UserEntity>> ListAsync(CancellationToken cancellationToken);
    
    Task<bool> DoBlockUserAsync(UserEntity user, CancellationToken cancellationToken);
    
    Task<bool> DoRestoreUserAsync(UserEntity user, CancellationToken cancellationToken);
  }
}