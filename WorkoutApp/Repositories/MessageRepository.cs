using System;
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
  public class MessageRepository : IMessageRepository
  {
    private readonly WorkoutDbContext _dbContext;
    private readonly IHubContext<HubClient, IHubClient> _hubContext; 
    
    public MessageRepository(
      WorkoutDbContext dbContext,
      IHubContext<HubClient, IHubClient> hubContext)
    {
      _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
      _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
    }
    
    public async Task DoBroadcastMessage(int receiverUserId) => 
      await _hubContext.Clients.User(receiverUserId.ToString()).BroadcastMessage();

    public async Task<ICollection<UserEntity>> DoListUsersWithMessageAsync(
      int currentUserId,
      CancellationToken cancellationToken)
    {
      var users = new List<UserEntity>();
      
      var senderUsers = await _dbContext.Messages
        .IgnoreQueryFilters()
        .Where(_ => _.TriggeredUserId == currentUserId)
        .Include(_ => _.SenderUser)
        .ThenInclude(_ => _.ProfilePicture)
        .OrderByDescending(_ => _.SentOn)
        .Select(_ => _.SenderUser)
        .ToListAsync(cancellationToken)
        .ConfigureAwait(false);
      
      var triggeredUsers = await _dbContext.Messages
        .IgnoreQueryFilters()
        .Where(_ => _.SenderUserId == currentUserId)
        .Include(_ => _.TriggeredUser)
        .ThenInclude(_ => _.ProfilePicture)
        .OrderByDescending(_ => _.SentOn)
        .Select(_ => _.TriggeredUser)
        .ToListAsync(cancellationToken)
        .ConfigureAwait(false);

      users.AddRange(senderUsers);
      users.AddRange(triggeredUsers);

      return users
        .Distinct()
        .ToList();
    }

    public async Task<ICollection<MessageEntity>> DoListAsync(
      int senderUserId,
      int triggeredUserId,
      CancellationToken cancellationToken)
    {
      return await _dbContext.Messages
        .IgnoreQueryFilters()
        .Where(_ => (_.SenderUserId == senderUserId && _.TriggeredUserId == triggeredUserId)
                      || (_.SenderUserId == triggeredUserId && _.TriggeredUserId == senderUserId))
        .Include(_ => _.SenderUser)
        .ThenInclude(_ => _.ProfilePicture)
        .Include(_ => _.TriggeredUser)
        .ThenInclude(_ => _.ProfilePicture)
        .OrderBy(_ => _.SentOn)
        .ToListAsync(cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task<MessageEntity> DoAddAsync(MessageEntity message, CancellationToken cancellationToken)
    {
      var createdMessage = await _dbContext.Messages
        .AddAsync(message, cancellationToken)
        .ConfigureAwait(false);

      await _dbContext
        .SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);

      return await _dbContext.Messages
        .Where(_ => _.Id == createdMessage.Entity.Id)
        .Include(_ => _.SenderUser)
        .ThenInclude(_ => _.ProfilePicture)
        .Include(_ => _.TriggeredUser)
        .ThenInclude(_ => _.ProfilePicture)
        .FirstOrDefaultAsync(cancellationToken)
        .ConfigureAwait(false);
    }
  }
}