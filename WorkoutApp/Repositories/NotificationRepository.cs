﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WorkoutApp.Abstractions;
using WorkoutApp.Data;
using WorkoutApp.Entities;
using WorkoutApp.Hubs;

namespace WorkoutApp.Repositories
{
  public class NotificationRepository : INotificationRepository
  {
    private readonly WorkoutDbContext _dbContext;
    private readonly IHubContext<HubClient, IHubClient> _hubContext; 
    
    public NotificationRepository(
      WorkoutDbContext dbContext,
      IHubContext<HubClient, IHubClient> hubContext)
    {
      _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
      _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
    }

    public async Task DoBroadcastNotifications(IEnumerable<string> userIds) => 
      await _hubContext.Clients.Users(userIds).BroadcastNotifications();
    

    public async Task DoBroadcastNotifications(int receiverUserId) => 
      await _hubContext.Clients.User(receiverUserId.ToString()).BroadcastNotifications();
    
    public async Task<ICollection<NotificationEntity>> DoListAsync(int id, CancellationToken cancellationToken)
    {
      return await _dbContext.Notifications
        .Where(_ => _.ReceivedUserId == id)
        .Include(_ => _.SentByUser)
        .ThenInclude(_ => _.ProfilePicture)
        .Include(_ => _.ReceivedUser)
        .OrderByDescending(_ => _.TriggeredOn)
        .ToListAsync(cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task DoAddAsync(NotificationEntity notification, CancellationToken cancellationToken)
    {
      await _dbContext.Notifications
        .AddAsync(notification, cancellationToken)
        .ConfigureAwait(false);

      await _dbContext
        .SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task DoDeleteAsync(int id, CancellationToken cancellationToken)
    {
      var notification = await _dbContext.Notifications
        .Where(_ => _.Id == id)
        .FirstOrDefaultAsync(cancellationToken)
        .ConfigureAwait(false);
      
      if (notification is null) {
        return;
      }
      
      notification.DeletedOn = DateTimeOffset.Now;
      
      await _dbContext
        .SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task DoDeleteAsync(int sentByUserId, int receivedUserId, NotificationType type, CancellationToken cancellationToken)
    {
      var notification = await _dbContext.Notifications
        .Where(_ => _.SentByUserId == sentByUserId
                    && _.ReceivedUserId == receivedUserId
                    && _.Type == type)
        .FirstOrDefaultAsync(cancellationToken)
        .ConfigureAwait(false);

      if (notification is null) {
        return;
      }
      
      notification.DeletedOn = DateTimeOffset.Now;
      
      await _dbContext
        .SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);
    }
  }
}