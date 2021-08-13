using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WorkoutApp.Entities;

namespace WorkoutApp.Abstractions
{
  public interface IFeedbackRepository
  {
    Task<ICollection<FeedbackEntity>> DoListAsync(CancellationToken cancellationToken);
    
    Task DoAddAsync(FeedbackEntity feedback, CancellationToken cancellationToken);
  }
}