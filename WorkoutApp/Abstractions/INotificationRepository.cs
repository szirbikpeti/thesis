using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WorkoutApp.Entities;

namespace WorkoutApp.Abstractions
{
  public interface INotificationRepository
  {
    Task DoBroadcastNotifications(int receivedUserId);
    
    Task DoBroadcastNotifications(IEnumerable<string> userIds);
    
    Task DoAddAsync(NotificationEntity notification, CancellationToken cancellationToken);
    
    Task DoDeleteAsync(int id, CancellationToken cancellationToken);
    
    Task DoDeleteAsync(int sentByUserId, int receivedUserId, NotificationType type, CancellationToken cancellationToken);
    
    Task<ICollection<NotificationEntity>> DoListAsync(int id, CancellationToken cancellationToken);
  }
}