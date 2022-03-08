using System.Net;
using System.Net.Mail;
using SendPulse;
using SendPulse.SDK;
using SendPulse.SDK.Models;

namespace CooperateAffairs.Logic
{
    public interface IMailService
	{
        public void SendEmailAsync();
	}
	public class MailService : IMailService
    {

        public void SendEmailAsync()
        {

            using (MailMessage mm = new MailMessage("akindekoayooluwa@gmail.com", "dekopaulayo@gmail.com"))
            {
                mm.Subject = "Test";
                mm.Body = "Test Body";
                mm.IsBodyHtml = false;
                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential NetworkCred = new NetworkCredential("dekopaulayo@gmail.com", "paulinto");
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = 587;
                    smtp.Send(mm);
                }
            }
        }
    }

	public class SendPulseMailService : IMailService
	{
		public void SendEmailAsync()
		{
            using (var sendPulse = new SendPulseService("Client ID", "Client Secret"))
			{
                sendPulse.SendEmailAsync(new EmailData()
                {
                    From = new EmailAddress
                    {
                        Name = "From Name",
                        Address = "From Address"
                    },
                    Subject = "Sample Subject",
                    Text = "Sample Body",
                    To = new List<EmailAddress>
                    {
                        new EmailAddress
                        {
                            Name = "To Name",
                            Address = "To Address"
                        }
                    }
                });
			}
		}
	}

	public class SendGridMailService : IMailService
	{
		public void SendEmailAsync()
		{
			throw new NotImplementedException();
		}
	}

	public class MailRequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
