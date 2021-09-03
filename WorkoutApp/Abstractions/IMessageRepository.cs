using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WorkoutApp.Entities;

namespace WorkoutApp.Abstractions
{
  public interface IMessageRepository
  {
    Task DoBroadcastMessage(int receivedUserId);
    
    Task<ICollection<UserEntity>> DoListUsersWithMessageAsync(int currentUserId, CancellationToken cancellationToken);
    
    Task<ICollection<MessageEntity>> DoListAsync(int senderUserId, int triggeredUserId, CancellationToken cancellationToken);
    
    Task<MessageEntity> DoAddAsync(MessageEntity message, CancellationToken cancellationToken);
  }
}