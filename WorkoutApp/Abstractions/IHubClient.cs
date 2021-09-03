using System.Threading.Tasks;

namespace WorkoutApp.Abstractions
{
  public interface IHubClient
  {
    Task BroadcastNotifications();
    
    Task BroadcastMessage();
  }
}