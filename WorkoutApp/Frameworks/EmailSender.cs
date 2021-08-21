using System.Net;
using System.Net.Mail;
using WorkoutApp.Abstractions;

namespace WorkoutApp.Frameworks
{
  public class EmailSender : IEmailSender
  {
    private const string FromMail = "workoutTrackingThesis@gmail.com";
    private const string FromPassword = "dhwfsvfwbpqxtsvi";
    
    public void SendEmail(string email, string subject, string htmlMessage)
    {
      MailMessage message = new() {
        From = new MailAddress(FromMail),
        Subject = subject
      };
      
      message.To.Add(new MailAddress(email));
      message.Body ="<html><body> " + htmlMessage + " </body></html>";
      message.IsBodyHtml = true;
 
      var smtpClient = new SmtpClient("smtp.gmail.com")
      {
        Port = 587,
        Credentials = new NetworkCredential(FromMail, FromPassword),
        EnableSsl = true,
      };
      smtpClient.Send(message);
    }
  }

}