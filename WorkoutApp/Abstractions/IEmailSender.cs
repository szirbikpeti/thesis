using System.Threading.Tasks;

namespace WorkoutApp.Abstractions
{
  public interface IEmailSender
  {
    void SendEmail(string email, string subject, string htmlMessage);
  }
}