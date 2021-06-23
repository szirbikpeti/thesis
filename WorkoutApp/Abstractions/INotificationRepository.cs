using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WorkoutApp.Entities;

namespace WorkoutApp.Abstractions
{
  public interface INotificationRepository
  {
    Task DoAddAsync(NotificationEntity notification, CancellationToken cancellationToken);
    
    Task<ICollection<NotificationEntity>> DoGetAsync(int id, CancellationToken cancellationToken);
  }
}